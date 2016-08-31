using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Meta;
using OsmSharp.Routing.Navigation.Language;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Routing.Navigation.Osm
{
  public class InstructionCarGenerator : InstructionGenerator<Instruction>
  {
    public InstructionCarGenerator(Route route, ILanguageReference languageReference)
      : this(route, languageReference, new InstructionCarGenerator.TryGetDelegateWithLanguageReference[4]
      {
        new InstructionCarGenerator.TryGetDelegateWithLanguageReference(InstructionCarGenerator.GetStartInstruction),
        new InstructionCarGenerator.TryGetDelegateWithLanguageReference(InstructionCarGenerator.GetStopInstruction),
        new InstructionCarGenerator.TryGetDelegateWithLanguageReference(InstructionCarGenerator.GetRoundaboutInstruction),
        new InstructionCarGenerator.TryGetDelegateWithLanguageReference(InstructionCarGenerator.GetTurnInstruction)
      })
    {
    }

    public InstructionCarGenerator(Route route, ILanguageReference languageReference, InstructionCarGenerator.TryGetDelegateWithLanguageReference[] getInstructionDelegates)
      : base(route, InstructionCarGenerator.GetInstructionDelegatesFrom(getInstructionDelegates, languageReference))
    {
    }

    public static List<Instruction> Generate(Route route, ILanguageReference languageReference)
    {
      InstructionCarGenerator instructionCarGenerator = new InstructionCarGenerator(route, languageReference);
      instructionCarGenerator.Run();
      if (instructionCarGenerator.HasSucceeded)
        return instructionCarGenerator.Instructions;
      return new List<Instruction>();
    }

    private static InstructionGenerator<Instruction>.TryGetDelegate GetInstructionDelegateFrom(InstructionCarGenerator.TryGetDelegateWithLanguageReference tryGetDelegate, ILanguageReference languageReference)
    {
      return (InstructionGenerator<Instruction>.TryGetDelegate) ((Route r, int i, out Instruction instruction) => tryGetDelegate(r, i, languageReference, out instruction));
    }

    private static InstructionGenerator<Instruction>.TryGetDelegate[] GetInstructionDelegatesFrom(InstructionCarGenerator.TryGetDelegateWithLanguageReference[] tryGetDelegates, ILanguageReference languageReference)
    {
      InstructionGenerator<Instruction>.TryGetDelegate[] tryGetDelegateArray = new InstructionGenerator<Instruction>.TryGetDelegate[tryGetDelegates.Length];
      for (int index = 0; index < tryGetDelegateArray.Length; ++index)
        tryGetDelegateArray[index] = InstructionCarGenerator.GetInstructionDelegateFrom(tryGetDelegates[index], languageReference);
      return tryGetDelegateArray;
    }

    public static int GetStartInstruction(Route r, int i, ILanguageReference languageReference, out Instruction instruction)
    {
      instruction = (Instruction) null;
      if (i != 0 || r.Segments.Count <= 0)
        return 0;
      DirectionEnum directionEnum = DirectionCalculator.Calculate(new GeoCoordinate((double) r.Segments[0].Latitude, (double) r.Segments[0].Longitude), new GeoCoordinate((double) r.Segments[1].Latitude, (double) r.Segments[1].Longitude));
      string str = languageReference[directionEnum.ToInvariantString()];
      instruction = new Instruction()
      {
        Text = string.Format(languageReference["Start {0}."], (object) str),
        Type = "start",
        Segment = 0
      };
      return 1;
    }

    public static int GetStopInstruction(Route r, int i, ILanguageReference languageReference, out Instruction instruction)
    {
      instruction = (Instruction) null;
      if (i != r.Segments.Count - 1 || r.Segments.Count <= 0)
        return 0;
      instruction = new Instruction()
      {
        Text = languageReference["Arrived at destination."],
        Type = "stop",
        Segment = r.Segments.Count - 1
      };
      return 1;
    }

    public static int GetTurnInstruction(Route r, int i, ILanguageReference languageReference, out Instruction instruction)
    {
      RelativeDirection relativeDirection = r.RelativeDirectionAt(i);
      if (relativeDirection != null)
      {
        bool flag = false;
        if (relativeDirection.Direction == RelativeDirectionEnum.StraightOn && r.Segments[i].SideStreets != null && r.Segments[i].SideStreets.Length >= 2)
          flag = true;
        else if (relativeDirection.Direction != RelativeDirectionEnum.StraightOn && relativeDirection.Direction != RelativeDirectionEnum.SlightlyLeft && (relativeDirection.Direction != RelativeDirectionEnum.SlightlyRight && r.Segments[i].SideStreets != null) && r.Segments[i].SideStreets.Length != 0)
          flag = true;
        if (flag)
        {
          string str = languageReference[relativeDirection.Direction.ToInvariantString()];
          string name = string.Empty;
          if (i + 1 < r.Segments.Count && r.Segments[i + 1].Tags != null && ((IEnumerable<RouteTags>) r.Segments[i + 1].Tags).Any<RouteTags>((Func<RouteTags, bool>) (x =>
          {
            if (!(x.Key == "name"))
              return false;
            name = x.Value;
            return true;
          })))
          {
            if (relativeDirection.Direction == RelativeDirectionEnum.StraightOn)
            {
              instruction = new Instruction()
              {
                Text = string.Format(languageReference["Go {0} on {1}."], new object[2]
                {
                  (object) str,
                  (object) name
                }),
                Type = "turn",
                Segment = i
              };
              return 1;
            }
            instruction = new Instruction()
            {
              Text = string.Format(languageReference["Turn {0} on {1}."], new object[2]
              {
                (object) str,
                (object) name
              }),
              Type = "turn",
              Segment = i
            };
            return 1;
          }
          if (relativeDirection.Direction == RelativeDirectionEnum.StraightOn)
            instruction = new Instruction()
            {
              Text = string.Format(languageReference["Go {0}."], (object) str),
              Type = "turn",
              Segment = i
            };
          instruction = new Instruction()
          {
            Text = string.Format(languageReference["Turn {0}."], (object) str),
            Type = "turn",
            Segment = i
          };
          return 1;
        }
      }
      instruction = (Instruction) null;
      return 0;
    }

    public static int GetRoundaboutInstruction(Route r, int i, ILanguageReference languageReference, out Instruction instruction)
    {
      if (r.Segments[i].Tags != null)
      {
        RouteTags[] tags1 = r.Segments[i].Tags;
        Func<RouteTags, bool> func1 = (Func<RouteTags, bool>) (x =>
        {
          if (x.Key == "junction")
            return x.Value == "roundabout";
          return false;
        });
        if (((IEnumerable<RouteTags>) tags1).Any<RouteTags>(func1) && i < r.Segments.Count)
        {
          if (r.Segments[i + 1].Tags != null)
          {
            RouteTags[] tags2 = r.Segments[i + 1].Tags;
            Func<RouteTags, bool> func2 = (Func<RouteTags, bool>) (x =>
            {
              if (x.Key == "junction")
                return x.Value == "roundabout";
              return false;
            });
            if (((IEnumerable<RouteTags>) tags2).Any<RouteTags>(func2))
              goto label_19;
          }
          int num1 = 1;
          int num2 = 1;
          for (int index = i - 1; index >= 0; --index)
          {
            ++num2;
            if (r.Segments[index].Tags != null)
            {
              RouteTags[] tags2 = r.Segments[index].Tags;
              Func<RouteTags, bool> func2 = (Func<RouteTags, bool>) (x =>
              {
                if (x.Key == "junction")
                  return x.Value == "roundabout";
                return false;
              });
              if (((IEnumerable<RouteTags>) tags2).Any<RouteTags>(func2))
              {
                if (r.Segments[index].SideStreets != null)
                  ++num1;
              }
              else
                break;
            }
            else
              break;
          }
          if (num1 == 1)
            instruction = new Instruction()
            {
              Text = string.Format(languageReference["Take the first exit at the next roundabout."], (object) num1),
              Type = "roundabout",
              Segment = i
            };
          else if (num1 == 2)
            instruction = new Instruction()
            {
              Text = string.Format(languageReference["Take the second exit at the next roundabout."], (object) num1),
              Type = "roundabout",
              Segment = i
            };
          else if (num1 == 3)
            instruction = new Instruction()
            {
              Text = string.Format(languageReference["Take the third exit at the next roundabout."], (object) num1),
              Type = "roundabout",
              Segment = i
            };
          else
            instruction = new Instruction()
            {
              Text = string.Format(languageReference["Take the {0}th exit at the next roundabout."], (object) num1),
              Type = "roundabout",
              Segment = i
            };
          return num2;
        }
      }
label_19:
      instruction = (Instruction) null;
      return 0;
    }

    public delegate int TryGetDelegateWithLanguageReference(Route route, int i, ILanguageReference languageReference, out Instruction instruction);
  }
}
