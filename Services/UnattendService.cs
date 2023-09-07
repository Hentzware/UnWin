using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace UnWin.Services
{
    public class UnattendService : IUnattendService
    {
        private bool _windowsPE = true;
        private bool _offlineServicing = true;
        private bool _generalize = true;
        private bool _specialize = true;
        private bool _auditSystem = true;
        private bool _auditUser = true;
        private bool _oobeSystem = true;

        public void SaveAutounattendXmlFile(string filePath)
        {
            using (var writer = XmlWriter.Create(filePath))
            {
                writer.WriteStartDocument();

                    writer.WriteStartElement("unattend");

                        writer.WriteAttributeString("xmlns", null, "urn:schemas-microsoft-com:unattend");

                        if (_windowsPE)
                        {
                            writer.WriteStartElement("settings");

                                writer.WriteAttributeString("pass", null, "windowsPE");

                            writer.WriteEndElement();
                        }

                        if (_offlineServicing)
                        {
                            writer.WriteStartElement("settings");

                                writer.WriteAttributeString("pass", null, "offlineServicing");

                            writer.WriteEndElement();
                        }

                        if (_generalize)
                        {
                            writer.WriteStartElement("settings");

                                writer.WriteAttributeString("pass", null, "generalze");

                            writer.WriteEndElement();
                        }

                        if (_specialize)
                        {
                            writer.WriteStartElement("settings");

                                writer.WriteAttributeString("pass", null, "specialize");

                            writer.WriteEndElement();
                        }

                        if (_auditSystem)
                        {
                            writer.WriteStartElement("settings");

                                writer.WriteAttributeString("pass", null, "auditSystem");

                            writer.WriteEndElement();
                        }

                        if (_auditUser)
                        {
                            writer.WriteStartElement("settings");

                                writer.WriteAttributeString("pass", null, "auditUser");

                            writer.WriteEndElement();
                        }

                        if (_oobeSystem)
                        {
                            writer.WriteStartElement("settings");

                                writer.WriteAttributeString("pass", null, "oobeSystem");

                            writer.WriteEndElement();
                        }

                        writer.WriteStartElement("component");

                            writer.WriteAttributeString("name", null, null);
                            writer.WriteAttributeString("processorArchitecture", null, "amd64");

                        writer.WriteEndElement();

                    writer.WriteEndElement();

                writer.WriteEndDocument();
            }
        }
    }
}
