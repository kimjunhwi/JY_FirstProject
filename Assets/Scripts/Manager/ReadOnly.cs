using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReadOnlys
{
	#region 블록 관련
	public enum E_BLOCKTYPE
	{
		E_NONE = 0,
        E_COUNT,
        E_MULTI,
        E_REFLECTION,
	}

	public enum E_BLOCK_COLOR_TYPE
	{
		E_NONE,
		E_RED,
		E_BLUE,
		E_GREEN,
	}

	public enum E_CHECK_BLOCK
	{
		E_FAILED,
		E_SUCCESS,
		E_YET,
	}

	#endregion

}
