using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Management;

namespace ROOT.CIMV2.Win32
{
    // Die Funktionen 'ShouldSerialize<PropertyName>' werden vom VS-Eigenschaftenbrowser verwendet, um zu überprüfen, ob eine Eigenschaft serialisiert werden muss. Die Funktionen werden für alle ValueType-Eigenschaften (Eigenschaften des Typs Int32, BOOL usw.) hinzugefügt, die nicht auf NULL festgelegt werden können. Die Funktionen verwenden die Is<PropertyName>Null-Funktion. Die Funktionen werden auch in der TypeConverter-Implementierung für die Eigenschaften verwendet, um den NULL-Wert zu überprüfen, damit für einen Drag & Drop-Vorgang in Visual Studio ein leerer Wert im Eigenschaftenbrowser angezeigt werden kann.
    // Die Funktionen 'Is<PropertyName>Null()' werden verwendet, um zu überprüfen, ob eine Eigenschaft NULL ist.
    // Die Funktionen 'Reset<PropertyName>' werden für Read/Write-Eigenschaften hinzugefügt. Die Funktionen werden vom VS-Designer im Eigenschaftenbrowser verwendet, um eine Eigenschaft auf NULL zusetzen.
    // Für jede Eigenschaft, die zur Klasse der WMI-Eigenschaft hinzugefügt wird, sind Attribute festgelegt, um das Verhalten im Visual Studio-Designer und auch 'TypeConverter' zu definieren.
    // Die Funktionen 'ToDateTime' und 'ToDmtfDateTime' zum Konvertieren von Datum bzw. Uhrzeit werden zu der Klasse hinzugefügt, um das DMTF-Datum in 'System.DateTime' und umgekehrt zu konvertieren.
    // Eine für die WMI-Klasse generierte EarlyBound-Klasse.Win32_Logicaldisk
    public class LogicalDisk : Component
    {
        // Private Eigenschaft, die den Namen der WMI-Klasse enthält, die diese Klasse erstellt hat.
        private const string CreatedClassName = "Win32_Logicaldisk";

        // Das aktuelle WMI-Objekt
        private readonly ManagementBaseObject _curObj;

        // Es sind unterschiedliche Konstruktorüberladungen aufgeführt, um die Instanz der Klasse mit einem WMI-Objekt zu initialisieren.
        public LogicalDisk()
            :
            this(null, null, null)
        {
        }

        public LogicalDisk(string keyDeviceId)
            :
            this(null, new ManagementPath(ConstructPath(keyDeviceId)), null)
        {
        }

        public LogicalDisk(ManagementScope mgmtScope, string keyDeviceId)
            :
            this(mgmtScope, new ManagementPath(ConstructPath(keyDeviceId)), null)
        {
        }

        public LogicalDisk(ManagementPath path, ObjectGetOptions getOptions)
            :
            this(null, path, getOptions)
        {
        }

        public LogicalDisk(ManagementScope mgmtScope, ManagementPath path)
            :
            this(mgmtScope, path, null)
        {
        }

        public LogicalDisk(ManagementPath path)
            :
            this(null, path, null)
        {
        }

        public LogicalDisk(ManagementScope mgmtScope, ManagementPath path, ObjectGetOptions getOptions)
        {
            if (path != null)
            {
                if (CheckIfProperClass(mgmtScope, path, getOptions) != true)
                {
                    throw new ArgumentException("Klassenname stimmt nicht überein.");
                }
            }

            ManagementObject privateLateBoundObject = new ManagementObject(mgmtScope, path, getOptions);
            _curObj = privateLateBoundObject;
        }

        public LogicalDisk(ManagementObject theObject)
        {
            if (CheckIfProperClass(theObject))
            {
                _curObj = theObject;
            }
            else
            {
                throw new ArgumentException("Klassenname stimmt nicht überein.");
            }
        }

        public LogicalDisk(ManagementBaseObject theObject)
        {
            if (CheckIfProperClass(theObject))
            {
                _curObj = theObject;
            }
            else
            {
                throw new ArgumentException("Klassenname stimmt nicht überein.");
            }
        }

        // Die Eigenschaft gibt den Namespace der WMI-Klasse zurück.

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ManagementClassName
        {
            get
            {
                string strRet = CreatedClassName;
                if (_curObj == null)
                {
                    return strRet;
                }

                strRet = (string)_curObj["__CLASS"];
                if (string.IsNullOrEmpty(strRet))
                {
                    strRet = CreatedClassName;
                }

                return strRet;
            }
        }

        // Eigenschaft für einen öffentlichen statischen Bereich, die von den verschiedenen Methoden verwendet wird.
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static ManagementScope StaticScope { get; set; } = null;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The Description property provides a textual description of the object. ")]
        public string Description => (string)_curObj["Description"];

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The Name property defines the label by which the object is known. When subclassed" +
                     ", the Name property can be overridden to be a Key property.")]
        public string Name => (string)_curObj["Name"];

        private bool CheckIfProperClass(ManagementScope mgmtScope, ManagementPath path, ObjectGetOptions optionsParam)
        {
            if (path != null
                && string.Compare(path.ClassName, ManagementClassName, true, CultureInfo.InvariantCulture) == 0)
            {
                return true;
            }

            return CheckIfProperClass(new ManagementObject(mgmtScope, path, optionsParam));
        }

        private bool CheckIfProperClass(ManagementBaseObject theObj)
        {
            if (theObj != null
                && string.Compare((string)theObj["__CLASS"], ManagementClassName, true,
                    CultureInfo.InvariantCulture) == 0)
            {
                return true;
            }

            Array parentClasses = (Array)theObj?["__DERIVATION"];
            if (parentClasses == null)
            {
                return false;
            }

            int count;
            for (count = 0; count < parentClasses.Length; count += 1)
            {
                if (string.Compare((string)parentClasses.GetValue(count), ManagementClassName, true,
                        CultureInfo.InvariantCulture) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static string ConstructPath(string keyDeviceId)
        {
            string strPath = "root\\cimv2:Win32_Logicaldisk";
            strPath = strPath + ".DeviceID=" + "\"" + keyDeviceId + "\"";
            return strPath;
        }

        // Unterschiedliche Überladungen der GetInstances()-Hilfe in den Enumerationsinstanzen der WMI-Klasse.

        public static LogicaldiskCollection GetInstances(ManagementScope mgmtScope, string condition) =>
            GetInstances(mgmtScope, condition, null);

        public static LogicaldiskCollection GetInstances(ManagementScope mgmtScope, string condition,
            string[] selectedProperties)
        {
            if (mgmtScope == null)
            {
                if (StaticScope == null)
                {
                    mgmtScope = new ManagementScope { Path = { NamespacePath = "root\\cimv2" } };
                }
                else
                {
                    mgmtScope = StaticScope;
                }
            }

            ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher(mgmtScope,
                new SelectQuery("Win32_Logicaldisk", condition, selectedProperties));
            EnumerationOptions enumOptions = new EnumerationOptions { EnsureLocatable = true };
            objectSearcher.Options = enumOptions;
            return new LogicaldiskCollection(objectSearcher.Get());
        }

        // Enumerationsimplementierung für die Instanzenenumeration der Klasse.
        public class LogicaldiskCollection : object, ICollection
        {
            private readonly ManagementObjectCollection _objectCollection;

            public LogicaldiskCollection(ManagementObjectCollection objCollection) => _objectCollection = objCollection;

            public int Count => _objectCollection.Count;

            public bool IsSynchronized => _objectCollection.IsSynchronized;

            public object SyncRoot => this;

            public void CopyTo(Array array, int index)
            {
                _objectCollection.CopyTo(array, index);
                int nCtr;
                for (nCtr = 0; nCtr < array.Length; nCtr += 1)
                {
                    array.SetValue(new LogicalDisk((ManagementObject)array.GetValue(nCtr)), nCtr);
                }
            }

            public IEnumerator GetEnumerator() => new LogicaldiskEnumerator(_objectCollection.GetEnumerator());

            public class LogicaldiskEnumerator : object, IEnumerator
            {
                private readonly ManagementObjectCollection.ManagementObjectEnumerator _objectEnumerator;

                public LogicaldiskEnumerator(ManagementObjectCollection.ManagementObjectEnumerator objEnum) =>
                    _objectEnumerator = objEnum;

                public object Current => new LogicalDisk((ManagementObject)_objectEnumerator.Current);

                public bool MoveNext() => _objectEnumerator.MoveNext();

                public void Reset() => _objectEnumerator.Reset();
            }
        }
    }
}
