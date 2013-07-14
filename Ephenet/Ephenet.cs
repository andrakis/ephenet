/**
 * Ephemeral Network
 * 
 * A software implementation of a networking protocol.
 * It attempts to emulate a physical ethernet network - cables, switches, routers, etc.
 * 
 * This is a small part of an overall infrastructure that will allow emulated
 * computer systems to talk to each other, whilst also providing metrics that
 * can be rendered by yet another system, so that one can visually see where
 * and how network traffic is flowing.
 * 
 * This library supplies only the connection between terminals. Hubs, switches,
 * routers and such will need to be programmed to interface with one or more
 * terminals, as in a physical network. These can be programmed in any language
 * that can speak this wire protocol.
 * 
 * Author: Julian "Andrakis" Thatcher (julian@noblesamurai.com)
 * Date: 2013/07/10
 * Version: 0
 */
 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ephenet.Interface;
using Ephenet;

namespace Ephenet.Sample
{
    class EphenetSample
    {
        static void Main(string[] args)
        {
			DumpTerminal t1 = new DumpTerminal();
			DumpTerminal t2 = new DumpTerminal();

			/*
			Timeout.Set(new TimeSpan(0, 0, 1), () =>
			{
				Console.WriteLine("Timeout after 5 seconds :)");
				Timeout.Set(new TimeSpan(0, 0, 1), () => {
					Console.WriteLine("And one more for good luck");
				});
			});

			Console.WriteLine("Waiting for timeout?");
			 */

			t1.PlugIn(t2);

			t1.Send(new Datagram(t1, t2, "Hello world".ToCharArray()));

			t1.PlugOut();

			Hub hub1 = new Hub(4);
			Hub hub2 = new Hub(4);

			// Plug in hub1 to hub2
			hub1.PlugIntoFree(hub2.GetFreeTerminal());
			// Plug in t1 to hub1
			t1.PlugIn(hub1.GetFreeTerminal());
			// ...
			t2.PlugIn(hub2.GetFreeTerminal());

			// Finally, send through t1, which should route through hub1, to hub2 and then to t2
			t1.Send(new Datagram(t1, t2, "Hello world".ToCharArray()));

			// Disconnect t2 and see what happens
			t2.PlugOut();
			t1.Send(new Datagram(t1, t2, "Hello world".ToCharArray()));

			t1.PlugOut();

			hub1.DebugVerbosity = HubImplementation.HubBase.DebugLevel.Minimal;
			hub2.DebugVerbosity = HubImplementation.HubBase.DebugLevel.Minimal;

			// Now try an arbitary number of hubs connected between t1 and t2.
			// We also randomly link to another hub, so that there can be more than
			// one pathway to the destination. This results in a terminal getting the
			// same message multiple times, a problem that the protocol layer will have
			// to deal with (or not.)
			Random rand = new Random();
			Hub[] hubs = new Hub[rand.Next(5, 10) + 1];
			for (int i = 0; i < hubs.Length; i++)
			{
				Hub hub = new Hub(rand.Next(4, 24));
				hub.DebugVerbosity = HubImplementation.HubBase.DebugLevel.Minimal;
				if (i == 0)
					hub.PlugIntoFree(hub1.GetFreeTerminal());
				else
					hub.PlugIntoFree(hubs[i - 1].GetFreeTerminal());
				if (i > 0 && rand.Next(0, 10) > 3)
				{
					// Randomly hook up to a random other hub
					Hub randHub = hubs[rand.Next(0, i - 1)];
					TerminalBase terminal = randHub.GetFreeTerminal();
					if (terminal != null)
					{
						Console.WriteLine("Random link {0} to {1}", hub.Address, randHub.Address);
						hub.PlugIntoFree(terminal);
					}
				}
				hubs[i] = hub;
			}
			hubs[hubs.Length - 1].PlugIntoFree(hub2.GetFreeTerminal());

			t1.PlugIn(hub1.GetFreeTerminal());
			t2.PlugIn(hub2.GetFreeTerminal());

			// Send should manage to route through
			t1.Send(new Datagram(t1, t2, "Hello world".ToCharArray()));

			Console.WriteLine("Finished. Press enter.");
			Console.ReadLine();
        }
    }
}
