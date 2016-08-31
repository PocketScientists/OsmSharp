using OsmSharp.Routing.Algorithms;
using System.Collections.Generic;

namespace OsmSharp.Routing.Navigation
{
  public class InstructionGenerator<T> : AlgorithmBase
  {
    private readonly Route _route;
    private readonly InstructionGenerator<T>.TryGetDelegate[] _tryGetInstructions;
    private readonly InstructionGenerator<T>.MergeDelegate _merge;
    private List<T> _instructions;
    private List<int> _instructionIndexes;
    private List<int> _instructionSizes;

    public List<T> Instructions
    {
      get
      {
        return this._instructions;
      }
    }

    public InstructionGenerator(Route route, InstructionGenerator<T>.TryGetDelegate[] tryGetInstructions)
      : this(route, tryGetInstructions, (InstructionGenerator<T>.MergeDelegate) null)
    {
    }

    public InstructionGenerator(Route route, InstructionGenerator<T>.TryGetDelegate[] tryGetInstructions, InstructionGenerator<T>.MergeDelegate merge)
    {
      this._route = route;
      this._tryGetInstructions = tryGetInstructions;
      this._merge = merge;
    }

    protected override void DoRun()
    {
      this._instructions = new List<T>();
      this._instructionIndexes = new List<int>();
      this._instructionSizes = new List<int>();
      for (int i = 0; i < this._route.Segments.Count; ++i)
      {
        for (int index1 = 0; index1 < this._tryGetInstructions.Length; ++index1)
        {
          T instruction;
          int num = this._tryGetInstructions[index1](this._route, i, out instruction);
          if (num > 0)
          {
            for (int index2 = this._instructions.Count - 1; index2 >= 0 && this._instructionIndexes[index2] > i - num; --index2)
            {
              this._instructions.RemoveAt(index2);
              this._instructionIndexes.RemoveAt(index2);
              this._instructionSizes.RemoveAt(index2);
            }
            this._instructions.Add(instruction);
            this._instructionIndexes.Add(i);
            this._instructionSizes.Add(num);
            this.HasSucceeded = true;
            break;
          }
        }
      }
      if (this._merge == null)
        return;
      for (int index = 1; index < this._instructions.Count; ++index)
      {
        T i;
        if (this._merge(this._route, this._instructions[index - 1], this._instructions[index], out i))
        {
          this._instructions[index - 1] = i;
          this._instructions.RemoveAt(index);
          --index;
        }
      }
    }

    public delegate int TryGetDelegate(Route route, int i, out T instruction);

    public delegate bool MergeDelegate(Route route, T i1, T i2, out T i);
  }
}
