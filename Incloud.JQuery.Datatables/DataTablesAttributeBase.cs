using System;
using System.Reflection;
using Incloud.JQuery.Datatables.Models;

namespace Incloud.JQuery.Datatables
{
    public abstract class DataTablesAttributeBase : Attribute
    {
        public abstract void ApplyTo(ColDef colDef, System.Reflection.PropertyInfo pi);
    }
}