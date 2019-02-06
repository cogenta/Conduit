# Conduit

Conduit is a pipeline processing framework that can handle synchronous/asynchronous/nested pipelines designed to be used with .Net Core/.Net Standard. 

At it's core, it uses `Microsoft.Extensions.DependencyInjection` to ensure that all parts of your pipeline are constructed properly, with each execution of the pipeline creating a new dependency injection scope to help clean up resources and prevent leaks, whilst facilitating other patterns, such as the unit of work pattern.

## Installation

You can install directly from nuget using the following command:

    > Install-Package Conduit

## Your first pipeline

To get started with a pipeline, you attach directly onto an `IServiceCollection` using the following snippet:

```c#
.AddPipeline<int>("MyFirstPipeline", builder =>
{
    builder.AddPhaseFromExpression(input => input + 1);
})
```

or 

```c#
.AddAsyncPipeline<int>("MyFirstPipeline", builder =>
{
    builder.AddPhaseFromExpression(input => input + 1);
})
```

`<int>` tells the add pipeline that both the input and output is going to be an `int` which allows for strongly typed decisions to be made in the rest of the pipeline. This is followed by a string, eg: `MyFirstPipeline`, which registers the pipeline with the given name.

Next we have have the builder which allows you to register a pipeline phase. A pipeline phase can be one of the following:

- An expression as shown above;
- A class that implements `IPipelinePhase`/`IAsyncPipelinePhase`, or;
- Another pipeline

Once it is registered in the container, you can access via one of two methods:

1. Injecting `IPipelineManager`/`IAsyncPipelineManager` and calling the `Get` method with the name that you wish to access.
2. Injecting `IPipeline<T>`/`IAsyncPipeline<T>` if you are using default pipelines (described below).

When you put all the pieces together, your program will look along the lines of this:

```c#
var services = new ServiceCollection()
                        .AddPipeline<int>("MyFirstPipeline", builder =>
                        {
                            builder.AddPhaseFromExpression(input => input + 1);
                        })
                        .BuildServiceProvider();

Console.WriteLine(services.GetRequiredService<IPipelineManager<int>>().Get(Constants.Pipeline1).Execute(5));
```

The output of the above program, should be: `6`

### Default pipelines (or nameless pipelines)

We recognised that you might not always have a complex setup that requires the use of named pipelines. Our solution to this is allowing you to define a default pipeline for a specified type.

To create a default pipeline, mirror the following code:

```c#
.AddDefaultPipeline<int>(builder =>
{
    builder.AddPhaseFromExpression(input => input + 1);
})
```

or;

```c#
.AddDefaultAsyncPipeline<int>(builder =>
{
    builder.AddPhaseFromExpression(input => input + 1);
})
```

As part of the default pipeline builder, you have access to all of the same stuff that you would when defining a named pipeline.

Once this is registered, you can access the pipeline in your code by injecting `IPipeline<T>`/`IAsyncPipeline<T>`.

_**Note**: You can have different default pipelines for your synchronos and asynchronos code paths.__

### Conditional execution of pipline phases

If you a part of your pipeline that you only occassionally want to run, you can supply a condition to each of the builder overloads which will only invoke the pipeline when the condition is met.

For more examples, see how the pipelines are constructed inside of the [samples](samples) folder.
