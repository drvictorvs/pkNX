using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pkNX.Containers.VFS;
using pkNX.Structures;

namespace pkNX.Game;

public static class GamePath
{
    private static GameVersion CurrentGame { get; set; }
    private static int CurrentLanguage { get; set; }

    public static void Initialize(GameVersion game, int language)
    {
        CurrentGame = game;
        CurrentLanguage = language;
    }

    public static FileSystemPath GetDirectoryPath(GameFile file)
    {
        return GetDirectoryPath(file, CurrentGame, CurrentLanguage);
    }

    public static FileSystemPath GetDirectoryPath(GameFile file, int language)
    {
        return GetDirectoryPath(file, CurrentGame, language);
    }

    public static FileSystemPath GetDirectoryPath(GameFile file, GameVersion game, int language)
    {
        if (file is GameFile.GameText or GameFile.StoryText)
            file += language + 1; // shift to localized language enum

        return game switch
        {
            GameVersion.GG => GetDirectoryPath_GG(file),
            GameVersion.SWSH => GetDirectoryPath_SWSH(file),
            GameVersion.PLA => GetDirectoryPath_PLA(file),
            GameVersion.SV => GetDirectoryPath_SV(file),

            _ => throw new NotSupportedException($"The selected game ({game}) is currently not mapped")
        };
    }

    private static FileSystemPath GetDirectoryPath_GG(GameFile file)
    {
        throw new NotImplementedException();
    }

    private static FileSystemPath GetDirectoryPath_SWSH(GameFile file)
    {
        throw new NotImplementedException();
    }

    // Search replace regex for conversion from GameFileMapping.cs:
    // Search for   : new\((\w+), (?:[^",]+, )?
    // Replace with : GameFile.$1 =>

    // Then to replace all ", " between each folder repeat the following:
    // Search for   : (?<=GameFile.\w+ => "[^"]+)", "
    // Replace with : /
    private static FileSystemPath GetDirectoryPath_PLA(GameFile file) => file switch
    {
        GameFile.GameText => throw new ArgumentException("Please shift language enum before calling this method."),
        GameFile.GameText0 => "/dump/message/dat/JPN/common/",
        GameFile.GameText1 => "/dump/message/dat/JPN_KANJI/common/",
        GameFile.GameText2 => "/dump/message/dat/English/common/",
        GameFile.GameText3 => "/dump/message/dat/French/common/",
        GameFile.GameText4 => "/dump/message/dat/Italian/common/",
        GameFile.GameText5 => "/dump/message/dat/German/common/",
        GameFile.GameText6 => "/dump/message/dat/Spanish/common/",
        GameFile.GameText7 => "/dump/message/dat/Korean/common/",
        GameFile.GameText8 => "/dump/message/dat/Simp_Chinese/common/",
        GameFile.GameText9 => "/dump/message/dat/Trad_Chinese/common/",

        GameFile.StoryText => throw new ArgumentException("Please shift language enum before calling this method."),
        GameFile.StoryText0 => "/dump/message/dat/JPN/script/",
        GameFile.StoryText1 => "/dump/message/dat/JPN_KANJI/script/",
        GameFile.StoryText2 => "/dump/message/dat/English/script/",
        GameFile.StoryText3 => "/dump/message/dat/French/script/",
        GameFile.StoryText4 => "/dump/message/dat/Italian/script/",
        GameFile.StoryText5 => "/dump/message/dat/German/script/",
        GameFile.StoryText6 => "/dump/message/dat/Spanish/script/",
        GameFile.StoryText7 => "/dump/message/dat/Korean/script/",
        GameFile.StoryText8 => "/dump/message/dat/Simp_Chinese/script/",
        GameFile.StoryText9 => "/dump/message/dat/Trad_Chinese/script/",

        _ => throw new NotSupportedException($"The selected file ({file}) is currently not mapped")
    };

    public static FileSystemPath GetDirectoryPath_SV(GameFile file) => file switch
    {
        GameFile.GameText => throw new ArgumentException("Please shift language enum before calling this method."),
        GameFile.GameText0 => "/dump/message/dat/JPN/common/",
        GameFile.GameText1 => "/dump/message/dat/JPN_KANJI/common/",
        GameFile.GameText2 => "/dump/message/dat/English/common/",
        GameFile.GameText3 => "/dump/message/dat/French/common/",
        GameFile.GameText4 => "/dump/message/dat/Italian/common/",
        GameFile.GameText5 => "/dump/message/dat/German/common/",
        GameFile.GameText6 => "/dump/message/dat/Spanish/common/",
        GameFile.GameText7 => "/dump/message/dat/Korean/common/",
        GameFile.GameText8 => "/dump/message/dat/Simp_Chinese/common/",
        GameFile.GameText9 => "/dump/message/dat/Trad_Chinese/common/",

        GameFile.StoryText => throw new ArgumentException("Please shift language enum before calling this method."),
        GameFile.StoryText0 => "/dump/message/dat/JPN/script/",
        GameFile.StoryText1 => "/dump/message/dat/JPN_KANJI/script/",
        GameFile.StoryText2 => "/dump/message/dat/English/script/",
        GameFile.StoryText3 => "/dump/message/dat/French/script/",
        GameFile.StoryText4 => "/dump/message/dat/Italian/script/",
        GameFile.StoryText5 => "/dump/message/dat/German/script/",
        GameFile.StoryText6 => "/dump/message/dat/Spanish/script/",
        GameFile.StoryText7 => "/dump/message/dat/Korean/script/",
        GameFile.StoryText8 => "/dump/message/dat/Simp_Chinese/script/",
        GameFile.StoryText9 => "/dump/message/dat/Trad_Chinese/script/",

        _ => throw new NotSupportedException($"The selected file ({file}) is currently not mapped")
    };
}
