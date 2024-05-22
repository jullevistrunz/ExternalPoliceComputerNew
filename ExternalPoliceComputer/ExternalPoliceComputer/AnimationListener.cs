﻿using CommonDataFramework.Modules.PedDatabase;
using Rage;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using PolicingRedefined.Interaction.Assets.PedAttributes;

namespace ExternalPoliceComputer {
    internal class AnimationListener {
        internal static Dictionary<Ped, Citations> data = new Dictionary<Ped, Citations>();
        
        internal static void ListenForAnimationFileChange() {
            FileSystemWatcher watcher = new FileSystemWatcher(Main.DataPath);
            watcher.Filter = "animation.data";
            watcher.EnableRaisingEvents = true;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += (object sender, FileSystemEventArgs e) => {
                if (e.ChangeType != WatcherChangeTypes.Changed) {
                    return;
                }

                NameValueCollection file = new NameValueCollection();
                
                try {
                    file = HttpUtility.ParseQueryString(File.ReadAllText($"{Main.DataPath}/animation.data"));
                } catch (Exception ex) {
                    Game.LogTrivial(ex.ToString());
                }

                switch (file["type"]) {
                    case "giveCitation":
                        Game.LogTrivial(file["name"]);

                        Ped ped = Main.Player.GetNearbyPeds(Main.MaxNumberOfNearbyPedsOrVehicles).FirstOrDefault(x => x.GetPedData().FullName == file["name"]);

                        if (ped == null) break;

                        Game.LogTrivial(ped.GetPedData().FullName);
                        
                        AddCitationToPed(ped, file["text"], int.Parse(file["fine"]), bool.Parse(file["isArrestable"]));
                        if(data.ContainsKey(ped)) data[ped].TransferCitations(ped);
                        break;
                }
            };
        }
        
        internal static void AddCitationToPed(Ped ped, string text, int fine, bool isArrestable) {
            if (data.ContainsKey(ped))
            {
                data[ped].AddCitation(text, fine, isArrestable);
            }
            else
            {
                data.Add(ped, new Citations());
                data[ped].AddCitation(text, fine, isArrestable);
            }
        }
    }
}
