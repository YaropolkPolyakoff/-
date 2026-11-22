# DanceFlow — WPF Dance Learning Application

A desktop application built with **C# / WPF / XAML** (.NET 8) for learning modern dance styles
through video lessons and 3D demonstration.

---

## Features

| Screen | Description |
|---|---|
| **Catalog** | Interactive card grid of 4 dance styles with hover animations |
| **Video Player** | Custom MediaElement player with speed control, mirror, A-B loop, step list |
| **3D Demo** | Viewport3D dancer figure with mouse-orbit camera, zoom, pose animations |

### Multimedia elements used

- **Raster graphics** – `.png` cover images per dance style via `<Image>` element
- **Vector graphics** – All icons, logo, player buttons and markers as `<Path>` / `<Canvas>` elements
- **Video files** – `.mp4` lessons via `<MediaElement>` with custom controls (`SpeedRatio`, seek, loop)
- **3D objects** – `<Viewport3D>` with `<GeometryModel3D>` dancer figure
- **2D animations** – Card scale-up on hover, page fade-in, mirror flip animation
- **3D animations** – Auto-rotate & pose tilt via `AxisAngleRotation3D` storyboards

---

## Project Structure

```
DanceApp/
├── App.xaml / App.xaml.cs           Application entry-point
├── MainWindow.xaml / .cs            Shell: left nav + content host
├── Models/
│   └── DanceStyle.cs                Data models + seed data
├── Converters/
│   └── Converters.cs                IValueConverter implementations
├── Views/
│   ├── CatalogView.xaml / .cs       Screen 1 — style cards
│   ├── VideoPlayerView.xaml / .cs   Screen 2 — video player + steps
│   └── Demo3DView.xaml / .cs        Screen 3 — 3D scene
└── Resources/
    ├── Images/
    │   ├── hiphop.png
    │   ├── breakdance.png
    │   ├── kpop.png
    │   └── highheels.png
    ├── Videos/
    │   └── sample.mp4               (place real lesson videos here)
    └── Styles/
        ├── Colors.xaml
        └── AppStyles.xaml
```

---

## Getting Started

### Prerequisites

- Visual Studio 2022 (with **.NET Desktop Development** workload)
- .NET 8 SDK

### Build & Run

```bash
cd DanceApp
dotnet build
dotnet run
```

Or open `DanceApp.sln` / `DanceApp.csproj` in **Visual Studio 2022 / Blend for Visual Studio 2022**
and press **F5**.

### Adding Media Assets

1. **Cover images** — place 240 × 300 `.png` files in `Resources/Images/`:
   - `hiphop.png`, `breakdance.png`, `kpop.png`, `highheels.png`
2. **Lesson videos** — place H.264 `.mp4` files in `Resources/Videos/`.
   Update the `VideoPath` values in `Models/DanceStyle.cs` to match.

> If image/video files are missing the app still runs:
> the placeholder panel is shown instead of the video,
> and cards fall back to a solid dark background.

---

## Controls Reference

### Video Player
| Control | Action |
|---|---|
| ▶ / ⏸ | Play / Pause |
| ← 10s / 10s → | Skip back / forward 10 seconds |
| 0.5× 0.75× 1× | Playback speed |
| Зеркало toggle | Flip video horizontally with animation |
| A–B Loop toggle | Click in video to set A, click again to set B; loops that segment |
| Step list row | Seek to that step's start time |

### 3D Scene
| Control | Action |
|---|---|
| Left-drag | Orbit camera 360° around dancer |
| Mouse wheel | Zoom in / out |
| Авто-вращение | Toggle continuous Y-axis spin |
| Движения buttons | Play a pose animation |
| ⟳ Сбросить камеру | Reset to default view |
