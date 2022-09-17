using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using Event = Exiled.Events.Handlers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Exiled.Events.EventArgs;

namespace AntiClip
{
    public class Plugin:Plugin<Config>
    {
        public override string Author { get; } = "sky";
        public override string Name { get; } = "Anticlip";
        public override string Prefix { get; } = "AC";
        public override Version Version { get; } = new Version(1,0,0);
        public override Version RequiredExiledVersion { get; } = new Version(5,3,0);

        public Plugin Singleton;

        public override void OnEnabled()
        {
            Singleton = this;
            Event.Server.RoundStarted += RoundStart;
            Event.Server.RoundEnded += RoundEnded;
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            Singleton = null;
            Event.Server.RoundStarted -= RoundStart;
            Event.Server.RoundEnded -= RoundEnded;
            base.OnDisabled();
        }
        public void RoundStart()
        {
            Timing.CallDelayed(0.1f, () => Timing.RunCoroutine(RegisterRoom(), "Register room"));
            Timing.RunCoroutine(AntiClip());
        }
        public void RoundEnded(RoundEndedEventArgs ev)
        {

        }

        public CoroutineHandle coroutine = new CoroutineHandle();
        public Dictionary<Player, RoomType> PlayerRoom = new Dictionary<Player, RoomType>();

        public IEnumerator<float> RegisterRoom()
        {
            for (; ; )
            {
                foreach (Player plr in Player.List)
                {
                    if (plr.CurrentRoom.Type != RoomType.Surface)
                    {
                        if (!PlayerRoom.TryGetValue(plr, out RoomType roomType))
                        {
                            PlayerRoom.Add(plr, plr.CurrentRoom.Type);
                        }
                        else
                        {
                            PlayerRoom.Remove(plr);
                            PlayerRoom.Add(plr, plr.CurrentRoom.Type);
                        }
                    }
                }
                yield return Timing.WaitForSeconds(0.1f);
            }
        }
        public IEnumerator<float> AntiClip()
        {
            for (; ; )
            {
                yield return Timing.WaitForSeconds(1f);
                foreach (Player player in Player.List)
                {
                    if (player.Position.y < -2000)
                    {
                        if (!Timing.IsRunning(coroutine))
                            coroutine = Timing.RunCoroutine(Alerte(player), $"{player.UserId} alerte");
                    }
                }
            }
        }

        public IEnumerator<float> Alerte(Player plr)
        {
            if (plr.Position.y < -2000)
            {
                plr.ShowHint($"<size={Singleton.Config.SizePercent}%><color=red>{Singleton.Config.Message}<color=green>3</color></color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
                if (plr.Position.y > -2000)
                {

                    Timing.RunCoroutine(Alerte(plr), $"{plr.UserId} alerte");
                    Timing.KillCoroutines($"{plr.UserId} alerte");
                }
                plr.ShowHint($"<size={Singleton.Config.SizePercent}%><color=red>{Singleton.Config.Message}<color=yellow>2</color></color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
                if (plr.Position.y > -2000)
                {

                    Timing.RunCoroutine(Alerte(plr), $"{plr.UserId} alerte");
                    Timing.KillCoroutines($"{plr.UserId} alerte");
                }
                plr.ShowHint($"<size={Singleton.Config.SizePercent}%><color=red>{Singleton.Config.Message}1</color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
                if (plr.Position.y > -2000)
                {

                    Timing.RunCoroutine(Alerte(plr), $"{plr.UserId} alerte");
                    Timing.KillCoroutines($"{plr.UserId} alerte");
                }
                plr.ShowHint($"<size={Singleton.Config.SizePercent}%><color=red>{Singleton.Config.Message}<color=white>0</color></color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
                if (plr.Position.y > -2000)
                {

                    Timing.RunCoroutine(Alerte(plr), $"{plr.UserId} alerte");
                    Timing.KillCoroutines($"{plr.UserId} alerte");
                }
                yield return Timing.WaitForSeconds(0.1f);
                if (PlayerRoom.TryGetValue(plr, out RoomType room))
                {
                    //Vector3 pos = Room.Get(RoomType.LczClassDSpawn).Position;
                    if (Room.Get(room))
                    {
                        Room roop = Room.Get(room);
                        plr.ResetSpeed();
                        plr.Position = new Vector3(roop.Position.x, roop.Position.y + 2, roop.Position.z);

                    }
                }
                else
                {
                    plr.Kill("ERROR 403 : ACCESS TO BACKROOM FORBIDDEN");
                    for (int i = 0; i < 3; i++)
                    {
                        ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
                        grenade.FuseTime = 0f;
                        grenade.SpawnActive(plr.Position);
                        yield return Timing.WaitForSeconds(0.1f);
                    }
                }
            }
            yield return Timing.WaitForSeconds(0.1f);
        }
    }
}
