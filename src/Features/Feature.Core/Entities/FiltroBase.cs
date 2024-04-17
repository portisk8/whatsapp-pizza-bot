using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feature.Core.Entities
{
    public class FiltroBase
    {
        public FiltroBase()
        {
            PageIndex = 1;
            PageSize = 9999;
        }

        public void SetMaxPageSize()
        {
            PageSize = int.MaxValue;
        }

        public string PalabrasABuscar { get; set; }

        public string ColumnaAOrdenar { get; set; }

        /// <summary>
        /// Numero de pagina
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// Cantidad de registros/filas por pagina
        /// </summary>
        public int PageSize { get; set; }

        public virtual void Reset()
        {
            PalabrasABuscar = string.Empty;
            PageIndex = 1;
            PageSize = 10;
        }
    }
}