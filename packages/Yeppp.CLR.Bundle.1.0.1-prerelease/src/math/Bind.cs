/*
 *                       Yeppp! library implementation
 *                   This file is auto-generated by Peach-Py,
 *        Portable Efficient Assembly Code-generator in Higher-level Python,
 *                  part of the Yeppp! library infrastructure
 * This file is part of Yeppp! library and licensed under the New BSD license.
 * See LICENSE.txt for the full text of the license.
 */

namespace Yeppp
{

	public partial class Math
	{

		internal static void Bind(NativeLibrary nativeLibrary)
		{
			Math.yepMath_Log_V64f_V64f = (yepMath_Log_V64f_V64f_Delegate)nativeLibrary.GetFunction("yepMath_Log_V64f_V64f", typeof(yepMath_Log_V64f_V64f_Delegate));
			Math.yepMath_Exp_V64f_V64f = (yepMath_Exp_V64f_V64f_Delegate)nativeLibrary.GetFunction("yepMath_Exp_V64f_V64f", typeof(yepMath_Exp_V64f_V64f_Delegate));
			Math.yepMath_Sin_V64f_V64f = (yepMath_Sin_V64f_V64f_Delegate)nativeLibrary.GetFunction("yepMath_Sin_V64f_V64f", typeof(yepMath_Sin_V64f_V64f_Delegate));
			Math.yepMath_Cos_V64f_V64f = (yepMath_Cos_V64f_V64f_Delegate)nativeLibrary.GetFunction("yepMath_Cos_V64f_V64f", typeof(yepMath_Cos_V64f_V64f_Delegate));
			Math.yepMath_Tan_V64f_V64f = (yepMath_Tan_V64f_V64f_Delegate)nativeLibrary.GetFunction("yepMath_Tan_V64f_V64f", typeof(yepMath_Tan_V64f_V64f_Delegate));
			Math.yepMath_EvaluatePolynomial_V32fV32f_V32f = (yepMath_EvaluatePolynomial_V32fV32f_V32f_Delegate)nativeLibrary.GetFunction("yepMath_EvaluatePolynomial_V32fV32f_V32f", typeof(yepMath_EvaluatePolynomial_V32fV32f_V32f_Delegate));
			Math.yepMath_EvaluatePolynomial_V64fV64f_V64f = (yepMath_EvaluatePolynomial_V64fV64f_V64f_Delegate)nativeLibrary.GetFunction("yepMath_EvaluatePolynomial_V64fV64f_V64f", typeof(yepMath_EvaluatePolynomial_V64fV64f_V64f_Delegate));
		}

	}

}
