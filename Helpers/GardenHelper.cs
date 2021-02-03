using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.RemoteWindows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using Clio.Utilities;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Navigation;
using ff14bot.NeoProfiles;
using ff14bot.Objects;
using ff14bot.Pathing;
using ff14bot.Pathing.Service_Navigation;
using ff14bot.RemoteWindows.GoldSaucer;
using GreyMagic;
using LlamaLibrary.Extensions;
using LlamaLibrary.Helpers;
using LlamaLibrary.Memory;
using LlamaLibrary.Properties;
using LlamaLibrary.RemoteAgents;
using LlamaLibrary.Retainers;
using LlamaLibrary.Structs;
using Newtonsoft.Json;
using TreeSharp;
using static ff14bot.RemoteWindows.Talk;
using static LlamaLibrary.Retainers.HelperFunctions;

namespace LlamaLibrary.Helpers
{	
	
    public static class GardenHelper
    {
		
		public static async Task<bool> GoGarden(uint AE)
		{
			Navigator.PlayerMover = new SlideMover();
			Navigator.NavigationProvider = new ServiceNavigationProvider();
			var house = WorldManager.AvailableLocations.FirstOrDefault(i => i.AetheryteId == AE);
			
			Log($"Teleporting to housing: (ZID: {house.ZoneId}, AID: {house.AetheryteId}) {house.Name}");
			await CommonTasks.Teleport(house.AetheryteId);

			Log("Waiting for zone to change");
			await Coroutine.Wait(20000, () => WorldManager.ZoneId == house.ZoneId);

			Log("Getting closest gardenning plot");

			var gardenPlot = GardenManager.Plants.FirstOrDefault();
			if (gardenPlot != null)
			{
				Log("Found nearby gardenning plot, approaching");
				await Navigation.FlightorMove(gardenPlot.Location);
				await GardenHelper.Main(); 
			}
			return true;
		}		
		
         public static bool AlwaysWater { get; set; }

        //public static void Log(string text, params object[] args) { Logger.Info(text, args); }

        public static async Task<bool> Main()
        {
            var watering = GardenManager.Plants.Where(r => !Blacklist.Contains(r) && r.Distance2D(Core.Player) < 5).ToArray();
            foreach (var plant in watering)
                //Water it if it needs it or if we have fertilized it 5 or more times.
                if (AlwaysWater || GardenManager.NeedsWatering(plant))
                {
                    var result = GardenManager.GetCrop(plant);
                    if (result != null)
                    {
                        Log($"Watering {result} {plant.ObjectId:X}");
						await Navigation.FlightorMove(plant.Location);
                        plant.Interact();
                        if (!await Coroutine.Wait(5000, () => Talk.DialogOpen)) continue;
                        Talk.Next();
                        if (!await Coroutine.Wait(5000, () => SelectString.IsOpen)) continue;
                        if (!await Coroutine.Wait(5000, () => SelectString.LineCount > 0)) continue;
                        if (SelectString.LineCount == 4)
                        {
                            SelectString.ClickSlot(1);
                            await Coroutine.Sleep(2300);
                        }
                        else
                        {
                            Log("Plant is ready to be harvested");
                            SelectString.ClickSlot(1);
                        }
                    }
                    else
                    {
                        Log($"GardenManager.GetCrop returned null {plant.ObjectId:X}");
                    }
                }
            var plants = GardenManager.Plants.Where(r => r.Distance2D(Core.Player) < 5).ToArray();
            foreach (var plant in plants)
            {
                var result = GardenManager.GetCrop(plant);
                if (result == null) continue;
                Log($"Fertilizing {GardenManager.GetCrop(plant)} {plant.ObjectId:X}");
				await Navigation.FlightorMove(plant.Location);
                plant.Interact();
                if (!await Coroutine.Wait(5000, () => Talk.DialogOpen)) continue;
                Talk.Next();
                if (!await Coroutine.Wait(5000, () => SelectString.IsOpen)) continue;
                if (!await Coroutine.Wait(5000, () => SelectString.LineCount > 0)) continue;
                if (SelectString.LineCount == 4)
                {
                    SelectString.ClickSlot(0);
                    if (await Coroutine.Wait(2000, () => GardenManager.ReadyToFertilize))
                    {
                        if (GardenManager.Fertilize() != FertilizeResult.Success) continue;
                        Log($"Plant with objectId {plant.ObjectId:X} was fertilized");
                        await Coroutine.Sleep(2300);
                    }
                    else
                    {
                        Log($"Plant with objectId {plant.ObjectId:X} not able to be fertilized, trying again later");
                    }
                }
                else
                {
                    Log("Plant is ready to be harvested");
                    SelectString.ClickSlot(1);
                }
            }
            return true;
        }
        
        public static void Log(string text)
        {
            Logging.Write(Colors.LawnGreen, text);
        }
    }
}