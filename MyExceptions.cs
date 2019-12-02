using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gis_gkh_Tomsk
{
	public class SignatureValidateException : Exception
	{
		private string ExceptionCode;
		public SignatureValidateException(string Code,string message = "", Exception innerException=null): base(message, innerException)
		{
			ExceptionCode = Code;
		}


	}
}
