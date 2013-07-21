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

namespace Ephenet
{
	/// <summary>
	/// Provides management of the Ephenet system.
	/// 
	/// Also initializes Ephenet systems.
	/// </summary>
    public static class Ephenet
    {
		//private static const 
    }
}
