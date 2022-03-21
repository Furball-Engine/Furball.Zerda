using Furball.Zerda;

ZerdaCabinet cabinet = new ZerdaCabinet();

cabinet.AddFile("V:/profiles.db");
cabinet.AddFile("V:/test.osu");

cabinet.Write("test.zrd");

ZerdaCabinet test = new ZerdaCabinet("test.zrd");

byte[] profilesDb = test.ReadFile("profiles.db");
byte[] osuFile = test.ReadFile("test.osu");

Console.ReadLine();