/*
 *  Copyright (c) 2013-2016, Cureos AB.
 *  All rights reserved.
 *  http://www.cureos.com
 *
 *	This file is part of Shim.Drawing.
 *
 *  Shim.Drawing is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Shim.Drawing is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Shim.Drawing.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Resources;
using System.Reflection;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle("Shim.Drawing")]
[assembly: AssemblyDescription("Shim.Drawing provides a minimal subset of System.Drawing for AForge.NET and Accord.NET.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Cureos AB")]
[assembly: AssemblyProduct("Shim.Drawing")]
[assembly: AssemblyCopyright("Copyright ©  2013-2016 Cureos AB")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("3.0.1")]
[assembly: AssemblyFileVersion("3.0.1.1")]

[assembly: InternalsVisibleTo("Accord.Imaging, PublicKey=0024000004800000940000000602000000240000525341310004000001000100039880a76dac76cddb9c85704c8a0e516773c28c0b202d9e0ae60b623b7bc554c7258bbf54ed6d98082964036109d4d970132b761f5b00a83079fbff2fbea283632a420ef5280dd2c5546e3f5da776191f7076a0966c06e7af21754fab55bdbdcddee5520632c3ebdc5908f6cdfb5b78d29123100f41faee0c29645e42455498")]
[assembly: InternalsVisibleTo("Accord.Vision, PublicKey=0024000004800000940000000602000000240000525341310004000001000100039880a76dac76cddb9c85704c8a0e516773c28c0b202d9e0ae60b623b7bc554c7258bbf54ed6d98082964036109d4d970132b761f5b00a83079fbff2fbea283632a420ef5280dd2c5546e3f5da776191f7076a0966c06e7af21754fab55bdbdcddee5520632c3ebdc5908f6cdfb5b78d29123100f41faee0c29645e42455498")]
[assembly: InternalsVisibleTo("Accord.Tests.Imaging, PublicKey=0024000004800000940000000602000000240000525341310004000001000100039880a76dac76cddb9c85704c8a0e516773c28c0b202d9e0ae60b623b7bc554c7258bbf54ed6d98082964036109d4d970132b761f5b00a83079fbff2fbea283632a420ef5280dd2c5546e3f5da776191f7076a0966c06e7af21754fab55bdbdcddee5520632c3ebdc5908f6cdfb5b78d29123100f41faee0c29645e42455498")]
[assembly: InternalsVisibleTo("AForge.Imaging, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c58482c3bcddd48d685aa62290cb0a4eea602ec5e649bda60486d75c1d73f70a3b1508ee59c0289e633178f75c35165624f772c0e3b71b34eb95cfb228eb315db3dea1b4c9a2838f58511031ea1b503efb129ec9ec2136e9d1dc031967c508f7450f0c3a64a68a118af7139db6c07a93b2f139935b0b1cbb66688e862efd4fda")]
[assembly: InternalsVisibleTo("AForge.Imaging.Formats, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b71de4d6138a06aac5f452678677d6ec30358794f40831500ff0ad452626232d5da34951accce4f9d5a4da30abb077f89abce68e800182bcc3fdefaf3cd84831269d4b1b366392547e0060f915f8053ec2e2fc90c4789d781a6f0616f8fdd3b64b2cfdda168be0e69923e03783aa18def5e9ec6d9dd814a5d8be768fb5f338eb")]
[assembly: InternalsVisibleTo("AForge.Vision, PublicKey=002400000480000094000000060200000024000052534131000400000100010065e77726b7e6213cc92c79cb73eccbbdd85b9763bbfc3a26a799ed7b0533f5ae91da766a1457cfeb6c525aef3d20f0b83d75376ffa07f8f71883dd9b1bab35f25945f6304530aa4120ad6a614a0ee5ad48b168259be27114e1d7554ecc27b84a834a30e6b05f5e7e8dc8375063241ddf5aa648f59d4bc7cdc9fa523aca3be3b1")]
[assembly: InternalsVisibleTo("AForge.Video, PublicKey=00240000048000009400000006020000002400005253413100040000010001002deebfed6a1753c5c8e4a9d2277d799e185882d96a7d671f5d6316ec5c9993366c3388ae6056482b0bd0510f822931f18c8962d9de058ef22a0c58635311eb746c00817fa1302c9b948d0e75f074142c6fb815be9a89279d7c5fc2ae8cc9c5a5e84777c6dce453161be4c0ca73906606c011b01064d07b736c1e14107d0b6bad")]
[assembly: InternalsVisibleTo("AForge.Imaging.Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c58482c3bcddd48d685aa62290cb0a4eea602ec5e649bda60486d75c1d73f70a3b1508ee59c0289e633178f75c35165624f772c0e3b71b34eb95cfb228eb315db3dea1b4c9a2838f58511031ea1b503efb129ec9ec2136e9d1dc031967c508f7450f0c3a64a68a118af7139db6c07a93b2f139935b0b1cbb66688e862efd4fda")]
[assembly: InternalsVisibleTo("AForge.Imaging.IPPrototyper, PublicKey=00240000048000009400000006020000002400005253413100040000010001009dc358a7789d9da1d025d27b395a0c3e3c688ff3d882fa4fc656816b17eac54344dec2922674ac45378cb7f0eb0cc08fbbb33ba8c483e45f6876c6c0b33e67a6b7727b5300fc90f5746bb43e7c8aad7f5cfee592991b0114e62ba542d3c652bba281f54d46f491827fedc5064ea0990e09aec35efb141d38231cbe675a6a5ed6")]
[assembly: InternalsVisibleTo("Accord.Extensions.Imaging.BitmapInterop, PublicKey=002400000480000094000000060200000024000052534131000400000100010017d77cae6475d12c12b98b2d5ef7faa7247c1c3a93e24af8548a83799520c0a4d5b290f3bfcd32a35df570436c1607224f42cc5eda97faa75d4c53f1d7de131411e56d35134806fdb93d1baf9982dbbfef056f765ab14f1b504e003f9affa83347e84d79b3ff30becc76f94564dd63c56e08c171f6126267620aef95c31490b7")]
