# ROSbag Reader for .NET
> Handle ROS bag files with ease

ROS Bag Reader allows you to process ROS bag files.


## Installation

ROS Message Parser for .Net is available as 
[NuGet Package](https://www.nuget.org/packages/RobSharper.Ros.BagReader/).

![](https://img.shields.io/nuget/v/RobSharper.Ros.BagReader.svg)


```
dotnet add package RobSharper.Ros.BagReader
``` 

### Supported .NET versions
 
* **.NET Standard 2.0**
    * .NET Core 2.0 or later
    * .NET Framework 4.6.1 or later
    * Mono 5.4 or later
    * Xamarin.iOS 10.14 or later
    * Xamarin.Mac 3.8 or later
    * Xamarin.Android 8.0 or later
    * Universal Windows Platform 10.0.16299 or later


## Usage

See [RobSharper.Ros.BagReader.Examples Project](RobSharper.Ros.BagReader.Examples/).

### Read messages from ROSBAG

```c#
public void Read_bag_and_message_data()
{
    // Open rosbag
    var bag = BufferBag.Create(ExampleBag.FilePath);

    // Create a RobSharper.MessageEssentiols Serializer
    var serializer = new RosMessageSerializer(new MessageTypeRegistry());

    // Filter messages
    var chatterMessages = bag.Messages
        .Where(message => message.Connection.DataTopic.Equals("/chatter"));

    // Iterate over filtered messages
    foreach (var message in chatterMessages)
    {
        var messageData = message.Message.Data;
        var stringMessage = serializer.Deserialize<StringMessage>(messageData);

        Console.WriteLine(stringMessage.Data);
    }
}
```

### Types of Bag readers

There are different `*Bag` classes for processing ROS bag files.
All implement the [`IBag`](RobSharper.Ros.BagReader/IBag.cs) interface.

For handling small bag files in memory use [`BufferBag`](RobSharper.Ros.BagReader/BufferBag.cs). 
You can load a BufferBag based on a file path, a stream or a `byte[]` buffer.

For handling large bag files use [`FileBag`](RobSharper.Ros.BagReader/FileBag.cs) or 
[`StreamBag`](RobSharper.Ros.BagReader/StreamBag.cs).
You can process bags based on a file path or a stream without loading it into memory.


## License

This project is licensed under the BSD 3-clause license. 
[Learn more](https://choosealicense.com/licenses/bsd-3-clause/)
