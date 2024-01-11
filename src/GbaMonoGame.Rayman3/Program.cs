using System.Text;

// Register encoding provider to be able to use Windows 1252
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
using var game = new GbaMonoGame.Rayman3.Rayman3();
game.Run();
