using HahnCargoTruckLoader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HahnCargoTruckLoader.Logic
{
  public class LoadingPlan
  {
    private readonly Dictionary<int, LoadingInstruction> instructions;
    private readonly Truck truck;
    private readonly List<Crate> crates;
    public LoadingPlan(Truck truck, List<Crate> crates)
    {
      this.truck = truck;
      this.crates = crates;
      instructions = [];
    }

    public Dictionary<int, LoadingInstruction> GetLoadingInstructions()
    {
      int stepnumber = 0;
      var sorteCrates = crates.OrderByDescending(c => c.Width * c.Height * c.Length).ToList();
      bool[,,] cargoSpace = new bool[truck.Width, truck.Height, truck.Length];
      foreach (var crate in sorteCrates)
      {
        bool placed = false;
        foreach (var orientation in GetPossibleOriotation(crate))
        {

          for (int x = 0; x <= truck.Width - orientation.Width; x++)
          {
            for (int y = 0; y <= truck.Height - orientation.Height; y++)
            {
              for (int z = 0; z <= truck.Length - orientation.Length; z++)
              {
                if (CanPlaceCrate(x, y, z, orientation, cargoSpace))
                {
                  PlaceCrate(x, y, z, orientation, cargoSpace);
                  instructions[crate.CrateID] = new LoadingInstruction
                  {
                    LoadingStepNumber = stepnumber++,
                    CrateId = crate.CrateID,
                    TopLeftX = x,
                    TopLeftY = y,
                    TurnHorizontal = orientation.TurnHorizontal,
                    TurnVertical = orientation.TurnVertical,
                  };
                  placed = true;
                  break;
                }
              }
              if (placed) break;
            }
            if (placed) break;
          }
          if (placed) break;
        }
        if (!placed) { throw new Exception($"Crate {crate.CrateID} could not be placed!"); };
      }

      return instructions;
    }
    public IEnumerable<(int Width, int Height, int Length, bool TurnHorizontal, bool TurnVertical)> GetPossibleOriotation(Crate crate)
    {
      yield return (crate.Width, crate.Height, crate.Length, false, false);
      yield return (crate.Length, crate.Height, crate.Width, true, false);
      yield return (crate.Width, crate.Length, crate.Height, false, true);
      yield return (crate.Length, crate.Width, crate.Height, true, false);
    }
    private bool CanPlaceCrate(int startX, int startY, int startZ, (int Width, int Height, int Length, bool TurnHorizontal, bool TurnVertical) orientation, bool[,,] cargoSpace)
    {
      if (startX + orientation.Width > truck.Width || startY + orientation.Height > truck.Height || startZ + orientation.Length > truck.Length)
      {
        return false;
      }

      for (int x = startX; x < startX + orientation.Width; x++)
      {
        for (int y = startY; y < startY + orientation.Height; y++)
        {
          for (int z = startZ; z < startZ + orientation.Length; z++)
          {
            if (cargoSpace[x, y, z])
            {
              return false;
            }
          }
        }
      }
      return true;
    }
    private void PlaceCrate(int startX, int startY, int startZ, (int Width, int Height, int Length, bool TurnHorizontal, bool TurnVertical) orientation, bool[,,] cargoSpace)
    {
      for (int x = startX; x < startX + orientation.Width; x++)
      {
        for (int y = startY; y < startY + orientation.Height; y++)
        {
          for (int z = startZ; z < startZ + orientation.Length; z++)
          {
            cargoSpace[x, y, z] = true;

          }
        }
      }
    }

  }
}
