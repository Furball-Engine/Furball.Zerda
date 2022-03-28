using System.Numerics;
using Furball.Zerda.Explorer.ImGuiData;
using ImGuiNET;
using Silk.NET.Windowing;

namespace Furball.Zerda.Explorer;

public class ExplorerWindow {
    private ZerdaCabinet _workingCabinet;
    private bool         _awaitingOpenFileDrop = false;

    public void LoadArchive(string filename) {
        if(File.Exists(filename))
            this._workingCabinet = new ZerdaCabinet(filename);
    }

    public unsafe ExplorerWindow(IWindow window) {
        var io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;

        fixed (uint* ptr = JetBrainsMono.Data) {
            io.Fonts.AddFontFromMemoryCompressedTTF((IntPtr)ptr, 133775, 16.0f);
        }

        var style = ImGui.GetStyle();
        style.FrameRounding    = 4.0f;
        style.WindowBorderSize = 0.0f;
        style.PopupBorderSize  = 0.0f;
        style.GrabRounding     = 4.0f;

        var colors = style.Colors;

        colors[(int)ImGuiCol.Text]                  = new Vector4(1.00f, 1.00f, 1.00f, 1.00f);
        colors[(int)ImGuiCol.TextDisabled]          = new Vector4(0.43f, 0.45f, 0.44f, 1.00f);
        colors[(int)ImGuiCol.WindowBg]              = new Vector4(0.09f, 0.09f, 0.09f, 0.94f);
        colors[(int)ImGuiCol.ChildBg]               = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
        colors[(int)ImGuiCol.PopupBg]               = new Vector4(0.08f, 0.08f, 0.08f, 0.94f);
        colors[(int)ImGuiCol.Border]                = new Vector4(0.20f, 0.20f, 0.20f, 0.50f);
        colors[(int)ImGuiCol.BorderShadow]          = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
        colors[(int)ImGuiCol.FrameBg]               = new Vector4(0.71f, 0.39f, 0.39f, 0.54f);
        colors[(int)ImGuiCol.FrameBgHovered]        = new Vector4(0.84f, 0.66f, 0.66f, 0.40f);
        colors[(int)ImGuiCol.FrameBgActive]         = new Vector4(0.84f, 0.66f, 0.66f, 0.67f);
        colors[(int)ImGuiCol.TitleBg]               = new Vector4(0.47f, 0.22f, 0.22f, 0.67f);
        colors[(int)ImGuiCol.TitleBgActive]         = new Vector4(0.47f, 0.22f, 0.22f, 1.00f);
        colors[(int)ImGuiCol.TitleBgCollapsed]      = new Vector4(0.47f, 0.22f, 0.22f, 0.67f);
        colors[(int)ImGuiCol.MenuBarBg]             = new Vector4(0.34f, 0.16f, 0.16f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarBg]           = new Vector4(0.02f, 0.02f, 0.02f, 0.53f);
        colors[(int)ImGuiCol.ScrollbarGrab]         = new Vector4(0.31f, 0.31f, 0.31f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarGrabHovered]  = new Vector4(0.41f, 0.41f, 0.41f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarGrabActive]   = new Vector4(0.51f, 0.51f, 0.51f, 1.00f);
        colors[(int)ImGuiCol.CheckMark]             = new Vector4(1.00f, 1.00f, 1.00f, 1.00f);
        colors[(int)ImGuiCol.SliderGrab]            = new Vector4(0.71f, 0.39f, 0.39f, 1.00f);
        colors[(int)ImGuiCol.SliderGrabActive]      = new Vector4(0.84f, 0.66f, 0.66f, 1.00f);
        colors[(int)ImGuiCol.Button]                = new Vector4(0.47f, 0.22f, 0.22f, 0.65f);
        colors[(int)ImGuiCol.ButtonHovered]         = new Vector4(0.71f, 0.39f, 0.39f, 0.65f);
        colors[(int)ImGuiCol.ButtonActive]          = new Vector4(0.20f, 0.20f, 0.20f, 0.50f);
        colors[(int)ImGuiCol.Header]                = new Vector4(0.71f, 0.39f, 0.39f, 0.54f);
        colors[(int)ImGuiCol.HeaderHovered]         = new Vector4(0.84f, 0.66f, 0.66f, 0.65f);
        colors[(int)ImGuiCol.HeaderActive]          = new Vector4(0.84f, 0.66f, 0.66f, 0.00f);
        colors[(int)ImGuiCol.Separator]             = new Vector4(0.43f, 0.43f, 0.50f, 0.50f);
        colors[(int)ImGuiCol.SeparatorHovered]      = new Vector4(0.71f, 0.39f, 0.39f, 0.54f);
        colors[(int)ImGuiCol.SeparatorActive]       = new Vector4(0.71f, 0.39f, 0.39f, 0.54f);
        colors[(int)ImGuiCol.ResizeGrip]            = new Vector4(0.71f, 0.39f, 0.39f, 0.54f);
        colors[(int)ImGuiCol.ResizeGripHovered]     = new Vector4(0.84f, 0.66f, 0.66f, 0.66f);
        colors[(int)ImGuiCol.ResizeGripActive]      = new Vector4(0.84f, 0.66f, 0.66f, 0.66f);
        colors[(int)ImGuiCol.Tab]                   = new Vector4(0.84f, 0.66f, 0.66f, 0.54f);
        colors[(int)ImGuiCol.TabHovered]            = new Vector4(0.84f, 0.66f, 0.66f, 0.66f);
        colors[(int)ImGuiCol.TabActive]             = new Vector4(0.84f, 0.66f, 0.66f, 0.54f);
        colors[(int)ImGuiCol.TabUnfocused]          = new Vector4(0.07f, 0.10f, 0.15f, 0.97f);
        colors[(int)ImGuiCol.TabUnfocusedActive]    = new Vector4(0.84f, 0.66f, 0.66f, 0.25f);
        colors[(int)ImGuiCol.PlotLines]             = new Vector4(0.61f, 0.61f, 0.61f, 1.00f);
        colors[(int)ImGuiCol.PlotLinesHovered]      = new Vector4(1.00f, 0.43f, 0.35f, 1.00f);
        colors[(int)ImGuiCol.PlotHistogram]         = new Vector4(0.90f, 0.70f, 0.00f, 1.00f);
        colors[(int)ImGuiCol.PlotHistogramHovered]  = new Vector4(1.00f, 0.60f, 0.00f, 1.00f);
        colors[(int)ImGuiCol.TextSelectedBg]        = new Vector4(0.26f, 0.59f, 0.98f, 0.35f);
        colors[(int)ImGuiCol.DragDropTarget]        = new Vector4(1.00f, 1.00f, 0.00f, 0.90f);
        colors[(int)ImGuiCol.NavHighlight]          = new Vector4(0.41f, 0.41f, 0.41f, 1.00f);
        colors[(int)ImGuiCol.NavWindowingHighlight] = new Vector4(1.00f, 1.00f, 1.00f, 0.70f);
        colors[(int)ImGuiCol.NavWindowingDimBg]     = new Vector4(0.80f, 0.80f, 0.80f, 0.20f);
        colors[(int)ImGuiCol.ModalWindowDimBg]      = new Vector4(0.80f, 0.80f, 0.80f, 0.35f);

        window.FileDrop += WindowOnFileDrop;
    }

    private void WindowOnFileDrop(string[] files) {
        if (this._awaitingOpenFileDrop) {
            this._workingCabinet = new ZerdaCabinet(files[0]);

            this._awaitingOpenFileDrop = false;
        }
    }

    public void Draw(double delta) {
        var viewport = ImGui.GetMainViewport();

        ImGui.SetNextWindowPos(viewport.WorkPos);
        ImGui.SetNextWindowSize(viewport.WorkSize);
        ImGui.SetNextWindowViewport(viewport.ID);

        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);

        ImGuiWindowFlags windowFlags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));

        ImGui.Begin("Main Window", windowFlags);

        ImGui.PopStyleVar(3);

        var dockspaceId = ImGui.GetID("MainWindow");



        ImGui.DockSpace(dockspaceId, Vector2.Zero, ImGuiDockNodeFlags.None);
        {
            if (ImGui.BeginMainMenuBar()) {
                if (ImGui.BeginMenu("File")) {
                    if (ImGui.MenuItem("New")) {
                        this._workingCabinet = new ZerdaCabinet() {
                            Filename = "New Cabinet"
                        };
                    }

                    if (ImGui.MenuItem("Open")) {
                        this._awaitingOpenFileDrop = true;
                    }

                    ImGui.EndMenu();
                }

                //Top Menu Bar
                ImGui.EndMainMenuBar();
            }

            if(this._awaitingOpenFileDrop)
                ImGui.OpenPopup("Open File");

            ImGui.SetNextWindowPos(ImGui.GetMainViewport().GetCenter(), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));

            bool uselessRef = true;

            if(ImGui.BeginPopupModal("Open File", ref uselessRef, ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoMove)) {
                if (!this._awaitingOpenFileDrop) {
                    ImGui.CloseCurrentPopup();
                }

                ImGui.Text("Please Drag a file onto this window to open.");
                ImGui.Separator();

                if (ImGui.Button("Cancel")) {
                    ImGui.CloseCurrentPopup();

                    this._awaitingOpenFileDrop = false;
                }

                ImGui.EndPopup();
            }

            ImGui.Begin("File Explorer", ImGuiWindowFlags.NoCollapse);
            {
                if (this._workingCabinet == null) {

                } else {
                    ImGui.Text(this._workingCabinet.Filename);
                    ImGui.Separator();

                    ImGui.Text($"{this._workingCabinet.Entries.Count} entires described.");

                    if(ImGui.BeginTable("FileTable", 3)) {

                        ImGui.TableSetupColumn("Filename");
                        ImGui.TableSetupColumn("Size");
                        ImGui.TableSetupColumn("Modification Date");

                        ImGui.TableHeadersRow();

                        for (int i = 0; i != this._workingCabinet.Entries.Count; i++) {
                            Entry currentEntry = this._workingCabinet.Entries[i];

                            ImGui.TableNextRow();

                            ImGui.TableNextRow();

                            ImGui.TableSetColumnIndex(0);
                            ImGui.Text(currentEntry.Filename);

                            ImGui.TableSetColumnIndex(1);

                            double kilobytes = Math.Round((double)currentEntry.Size / 1024.0, 2);

                            ImGui.Text($"{kilobytes}kb");

                            ImGui.TableSetColumnIndex(2);
                            ImGui.Text(DateTimeOffset.FromUnixTimeSeconds(currentEntry.ModificationDate).ToString());
                        }

                        ImGui.EndTable();
                    }
                }
            }

            //File Explorer
            ImGui.End();
        }

        //Dockspace
        ImGui.End();
    }
}
