SyntaxHighlighter.brushes.MSIL = function()
{
	var keywords =	'nop break ldarg.0 ldarg.1 ldarg.2 ldarg.3 ldloc.0 ldloc.1 ldloc.2 ldloc.3' +
					'stloc.0 stloc.1 stloc.2 stloc.3 ldarg.s ldarga.s starg.s ldloc.s ldloca.s stloc.s' +
					'ldnull ldc.i4.m1 ldc.i4.0 ldc.i4.1 ldc.i4.2 ldc.i4.3 ldc.i4.4 ldc.i4.5 ldc.i4.6 ldc.i4.7' +
					'ldc.i4.8 ldc.i4.s ldc.i4 ldc.i8 ldc.r4 ldc.r8 dup pop jmp call' +
					'calli ret br.s brfalse.s brtrue.s beq.s bge.s bgt.s ble.s blt.s' +
					'bne.un.s bge.un.s bgt.un.s ble.un.s blt.un.s br brfalse brtrue beq bge' +
					'bgt ble blt bne.un bge.un bgt.un ble.un blt.un switch ldind.i1' +
					'ldind.u1 ldind.i2 ldind.u2 ldind.i4 ldind.u4 ldind.i8 ldind.i ldind.r4 ldind.r8 ldind.ref' +
					'stind.ref stind.i1 stind.i2 stind.i4 stind.i8 stind.r4 stind.r8 add sub mul' +
					'div div.un rem rem.un and or xor shl shr shr.un' +
					'neg not conv.i1 conv.i2 conv.i4 conv.i8 conv.r4 conv.r8 conv.u4 conv.u8' +
					'callvirt cpobj ldobj ldstr newobj castclass isinst conv.r.un unbox throw' +
					'ldfld ldflda stfld ldsfld ldsflda stsfld stobj conv.ovf.i1.un conv.ovf.i2.un conv.ovf.i4.un' +
					'conv.ovf.i8.un conv.ovf.u1.un conv.ovf.u2.un conv.ovf.u4.un conv.ovf.u8.un conv.ovf.i.un conv.ovf.u.un box newarr ldlen' +
					'ldelema ldelem.i1 ldelem.u1 ldelem.i2 ldelem.u2 ldelem.i4 ldelem.u4 ldelem.i8 ldelem.i ldelem.r4' +
					'ldelem.r8 ldelem.ref stelem.i stelem.i1 stelem.i2 stelem.i4 stelem.i8 stelem.r4 stelem.r8 stelem.ref' +
					'ldelem stelem unbox.any conv.ovf.i1 conv.ovf.u1 conv.ovf.i2 conv.ovf.u2 conv.ovf.i4 conv.ovf.u4 conv.ovf.i8' +
					'conv.ovf.u8 refanyval ckfinite mkrefany ldtoken conv.u2 conv.u1 conv.i conv.ovf.i conv.ovf.u' +
					'add.ovf add.ovf.un mul.ovf mul.ovf.un sub.ovf sub.ovf.un endfinally leave leave.s stind.i' +
					'conv.u prefix7 prefix6 prefix5 prefix4 prefix3 prefix2 prefix1 prefixref arglist' +
					'ceq cgt cgt.un clt clt.un ldftn ldvirtftn ldarg ldarga starg' +
					'ldloc ldloca stloc localloc endfilter unaligned. volatile. tail. initobj constrained.' +
					'cpblk initblk rethrow sizeof refanytype readonly.' +

					'.class .method' +

					'static public private family' +
					'final specialname virtual strict abstract assembly famandassem famorassem privatescope hidebysig newslot rtspecialname unmanagedexp' + 
					'reqsecobj flags' +

					'pinvokeimpl nomangle ansi unicode autochar lasterr winapi cdecl stdcall thiscall fastcall bestfit charmaperror' +
					'.ctor .cctor native cil optil managed unmanaged forwardref preservesig runtime internalcall synchronized noinlining nooptimization' +
					'.locals .emitbyte .maxstack .entrypoint .export .vtentry .override param .try filter catch finally fault handler .data tls';

	function fixComments(match, regexInfo)
	{
		var css = (match[0].indexOf("///") == 0)
			? 'color1'
			: 'comments'
			;
			
		return [new SyntaxHighlighter.Match(match[0], match.index, css)];
	}

	this.regexList = [
		{ regex: SyntaxHighlighter.regexLib.singleLineCComments,	func : fixComments },		// one line comments
		{ regex: SyntaxHighlighter.regexLib.multiLineCComments,		css: 'comments' },			// multiline comments
		{ regex: SyntaxHighlighter.regexLib.doubleQuotedString,		css: 'string' },			// strings
		{ regex: SyntaxHighlighter.regexLib.singleQuotedString,		css: 'string' },			// strings
		{ regex: /^\s*#.*/gm,										css: 'preprocessor' },		// preprocessor tags like #region and #endregion
		{ regex: new RegExp(this.getKeywords(keywords), 'gm'),		css: 'keyword' },			// il keyword
		];		
	
};

SyntaxHighlighter.brushes.MSIL.prototype= new SyntaxHighlighter.Highlighter();
SyntaxHighlighter.brushes.MSIL.aliases	= ['il', 'cil', 'msil'];