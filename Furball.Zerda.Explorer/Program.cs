using Furball.Zerda.Explorer;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using IWindow window = Window.Create(WindowOptions.Default);

ImGuiController imGuiController = null;
GL              gl              = null;
IInputContext   inputContext    = null;

ExplorerWindow explorerWindow = null;

window.Load += delegate {
	imGuiController = new ImGuiController(gl = window.CreateOpenGL(), window, inputContext = window.CreateInput(), delegate {
		explorerWindow = new ExplorerWindow(window!);

		if (args.Length != 0)
			explorerWindow.LoadArchive(args[0]);
	});
};

window.FramebufferResize += delegate(Vector2D<int> newSize) {
	gl.Viewport(newSize);
};

window.Render += delegate(double delta) {
	imGuiController!.Update((float)delta);

	gl.Clear(ClearBufferMask.ColorBufferBit);

	explorerWindow!.Draw(delta);

	imGuiController.Render();
};

window.Closing += delegate {
	imGuiController?.Dispose();
	inputContext?.Dispose();
	gl?.Dispose();
};

window.Run();
