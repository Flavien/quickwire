// Copyright 2021 Flavien Charlon
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Quickwire;

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

internal class DelegateCompiler
{
    public delegate void Setter(object? target, object? value);

    public delegate object? Factory(object?[] arguments);

    public delegate object Constructor(object?[] arguments);

    public static Setter CreateSetter(Type type, MethodInfo setter)
    {
        ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "target");
        ParameterExpression valueParameter = Expression.Parameter(typeof(object), "value");

        MethodCallExpression call = Expression.Call(
            Expression.Convert(instanceParameter, type),
            setter,
            Expression.Convert(valueParameter, setter.GetParameters()[0].ParameterType));

        Expression<Setter> lambda = Expression.Lambda<Setter>(
            call,
            instanceParameter,
            valueParameter);

        return lambda.Compile();
    }

    public static Factory CreateFactory(MethodInfo method)
    {
        ParameterExpression argumentsParameter = Expression.Parameter(typeof(object[]), "arguments");

        MethodCallExpression call = Expression.Call(
            null,
            method,
            CreateParameterExpressions(method, argumentsParameter));

        Expression<Factory> lambda = Expression.Lambda<Factory>(
            Expression.Convert(call, typeof(object)),
            argumentsParameter);

        return lambda.Compile();
    }

    public static Constructor CreateConstructor(ConstructorInfo constructor)
    {
        ParameterExpression argumentsParameter = Expression.Parameter(typeof(object[]), "arguments");

        NewExpression call = Expression.New(
            constructor,
            CreateParameterExpressions(constructor, argumentsParameter));

        Expression<Constructor> lambda = Expression.Lambda<Constructor>(
            Expression.Convert(call, typeof(object)),
            argumentsParameter);

        return lambda.Compile();
    }

    private static Expression[] CreateParameterExpressions(MethodBase method, Expression argumentsParameter)
    {
        return method.GetParameters()
            .Select((parameter, index) =>
                Expression.Convert(
                    Expression.ArrayIndex(argumentsParameter, Expression.Constant(index)),
                    parameter.ParameterType))
            .ToArray();
    }
}
