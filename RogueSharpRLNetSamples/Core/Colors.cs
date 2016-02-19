using RLNET;

namespace RogueSharpRLNetSamples.Core
{
   public static class Colors
   {
      public static RLColor DoorBackground = Swatch.ComplimentDarkest;
      public static RLColor Door = Swatch.ComplimentLighter;
      public static RLColor DoorBackgroundFov = Swatch.ComplimentDarker;
      public static RLColor DoorFov = Swatch.ComplimentLightest;
      public static RLColor FloorBackground = RLColor.Black;
      public static RLColor Floor = Swatch.AlternateDarkest;
      public static RLColor FloorBackgroundFov = Swatch.DbDark;
      public static RLColor FloorFov = Swatch.Alternate;
      public static RLColor WallBackground = Swatch.SecondaryDarkest;
      public static RLColor Wall = Swatch.Secondary;
      public static RLColor WallBackgroundFov = Swatch.SecondaryDarker;
      public static RLColor WallFov = Swatch.SecondaryLighter;
      public static RLColor GoblinColor = RLColor.Green;
      public static RLColor KoboldColor = new RLColor( 255, 165, 0 );
      public static RLColor OozeColor = new RLColor( 102, 205, 170 );
      public static RLColor Player = Swatch.DbLight;
      public static RLColor InventoryHeading = Swatch.DbLight;
   }

   public static class Swatch
   {
      // http://paletton.com/#uid=73d0u0k5qgb2NnT41jT74c8bJ8X

      public static RLColor PrimaryLightest = new RLColor( 110, 121, 119 );
      public static RLColor PrimaryLighter = new RLColor( 88, 100, 98 );
      public static RLColor Primary = new RLColor( 68, 82, 79 );
      public static RLColor PrimaryDarker = new RLColor( 48, 61, 59 );
      public static RLColor PrimaryDarkest = new RLColor( 29, 45, 42 );

      public static RLColor SecondaryLightest = new RLColor( 116, 120, 126 );
      public static RLColor SecondaryLighter = new RLColor( 93, 97, 105 );
      public static RLColor Secondary = new RLColor( 72, 77, 85 );
      public static RLColor SecondaryDarker = new RLColor( 51, 56, 64 );
      public static RLColor SecondaryDarkest = new RLColor( 31, 38, 47 );

      public static RLColor AlternateLightest = new RLColor( 190, 184, 174 );
      public static RLColor AlternateLighter = new RLColor( 158, 151, 138 );
      public static RLColor Alternate = new RLColor( 129, 121, 107 );
      public static RLColor AlternateDarker = new RLColor( 97, 89, 75 );
      public static RLColor AlternateDarkest = new RLColor( 71, 62, 45 );

      public static RLColor ComplimentLightest = new RLColor( 190, 180, 174 );
      public static RLColor ComplimentLighter = new RLColor( 158, 147, 138 );
      public static RLColor Compliment = new RLColor( 129, 116, 107 );
      public static RLColor ComplimentDarker = new RLColor( 97, 84, 75 );
      public static RLColor ComplimentDarkest = new RLColor( 71, 56, 45 );

      // http://pixeljoint.com/forum/forum_posts.asp?TID=12795

      public static RLColor DbDark = new RLColor( 20, 12, 28 );
      public static RLColor DbOldBlood = new RLColor( 68, 36, 52 );
      public static RLColor DbDeepWater = new RLColor( 48, 52, 109 );
      public static RLColor DbOldStone = new RLColor( 78, 74, 78 );
      public static RLColor DbWood = new RLColor( 133, 76, 48 );
      public static RLColor DbVegetation = new RLColor( 52, 101, 36 );
      public static RLColor DbBlood = new RLColor( 208, 70, 72 );
      public static RLColor DbStone = new RLColor( 117, 113, 97 );
      public static RLColor DbWater = new RLColor( 89, 125, 206 );
      public static RLColor DbBrightWood = new RLColor( 210, 125, 44 );
      public static RLColor DbMetal = new RLColor( 133, 149, 161 );
      public static RLColor DbGrass = new RLColor( 109, 170, 44 );
      public static RLColor DbSkin = new RLColor( 210, 170, 153 );
      public static RLColor DbSky = new RLColor( 109, 194, 202 );
      public static RLColor DbSun = new RLColor( 218, 212, 94 );
      public static RLColor DbLight = new RLColor( 222, 238, 214 );
   }
}