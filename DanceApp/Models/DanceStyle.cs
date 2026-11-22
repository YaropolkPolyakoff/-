using System.Collections.Generic;

namespace DanceApp.Models
{
    /// <summary>
    /// Represents a single dance style / course available in the catalog.
    /// </summary>
    public class DanceStyle
    {
        public string Name        { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int    LessonCount { get; set; }
        public string ImagePath   { get; set; } = string.Empty;
        public string AccentColor { get; set; } = "#FF6C3DE3";
        public List<DanceLesson> Lessons { get; set; } = new();
    }

    /// <summary>
    /// A single lesson inside a dance style.
    /// </summary>
    public class DanceLesson
    {
        public int    Number      { get; set; }
        public string Title       { get; set; } = string.Empty;
        public string VideoPath   { get; set; } = string.Empty;
        public List<DanceStep> Steps { get; set; } = new();
    }

    /// <summary>
    /// A single movement step inside a lesson.
    /// </summary>
    public class DanceStep
    {
        public int    Number      { get; set; }
        public string Name        { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double StartTime   { get; set; }  // seconds
        public double EndTime     { get; set; }  // seconds
        public bool   IsActive    { get; set; }
    }

    /// <summary>
    /// Static seed data used when real media files are not present.
    /// </summary>
    public static class DanceData
    {
        public static List<DanceStyle> GetStyles() => new()
        {
            new DanceStyle
            {
                Name = "Hip-Hop", Description = "Уличный стиль с историей", LessonCount = 12,
                ImagePath = "pack://application:,,,/Resources/Images/hiphop.png", AccentColor = "#FFFF6B35",
                Lessons = GetLessons("Hip-Hop")
            },
            new DanceStyle
            {
                Name = "Breakdance", Description = "Акробатика и фриз-стили", LessonCount = 10,
                ImagePath = "pack://application:,,,/Resources/Images/breakdance.png", AccentColor = "#FF3AFFD8",
                Lessons = GetLessons("Breakdance")
            },
            new DanceStyle
            {
                Name = "K-Pop Cover Dance", Description = "Корейские чарт-хиты", LessonCount = 15,
                ImagePath = "pack://application:,,,/Resources/Images/kpop.png", AccentColor = "#FFFF3CAC",
                Lessons = GetLessons("K-Pop Cover Dance")
            },
            new DanceStyle
            {
                Name = "High Heels", Description = "Пластика и женская сила", LessonCount = 8,
                ImagePath = "pack://application:,,,/Resources/Images/highheels.png", AccentColor = "#FFA855F7",
                Lessons = GetLessons("High Heels")
            },
        };

        private static List<DanceLesson> GetLessons(string style) => new()
        {
            new DanceLesson
            {
                Number = 1, Title = $"{style} — Базовые шаги",
                VideoPath = "pack://application:,,,/Resources/Videos/sample.mp4",
                Steps = new List<DanceStep>
                {
                    new() { Number=1, Name="Разминка",        Description="Разогрев мышц и суставов",  StartTime=0,   EndTime=15  },
                    new() { Number=2, Name="Базовый bounce",  Description="Пружинистое движение корпуса", StartTime=15,  EndTime=35  },
                    new() { Number=3, Name="Шаг вправо",      Description="Перенос веса на правую ногу",  StartTime=35,  EndTime=55  },
                    new() { Number=4, Name="Шаг влево",       Description="Перенос веса на левую ногу",   StartTime=55,  EndTime=75  },
                    new() { Number=5, Name="Комбинация 1-4",  Description="Соединяем все шаги вместе",    StartTime=75,  EndTime=120 },
                }
            },
            new DanceLesson
            {
                Number = 2, Title = $"{style} — Работа рук",
                VideoPath = "pack://application:,,,/Resources/Videos/sample.mp4",
                Steps = new List<DanceStep>
                {
                    new() { Number=1, Name="Позиция рук",     Description="Исходное положение",          StartTime=0,   EndTime=20  },
                    new() { Number=2, Name="Волна руками",    Description="Плавная волна от плеча",       StartTime=20,  EndTime=50  },
                    new() { Number=3, Name="Clap & hit",      Description="Акцентированные удары",        StartTime=50,  EndTime=80  },
                }
            },
        };
    }
}
