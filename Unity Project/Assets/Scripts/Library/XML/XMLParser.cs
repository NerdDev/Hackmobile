using System.Collections;

namespace XML
{
    internal class XMLParser
    {
        private char LA = '<';
        private char RA = '>';
        private char SPACE = ' ';
        private char QUOTE = '"';
        private char QUOTE2 = '\'';
        private char SLASH = '/';
        private char QMARK = '?';
        private char EQUALS = '=';
        private char EXCLAMATION = '!';
        private char DASH = '-';
        private char SQR = ']';

        public XMLNode Parse(string content)
        {
            XMLNode rootNode = new XMLNode("#document");
            rootNode.setText("");

            bool inElement = false;
            bool collectNodeName = false;
            bool collectAttributeName = false;
            bool collectAttributeValue = false;
            bool quoted = false;
            string attName = "";
            string attValue = "";
            string nodeName = "";
            string textValue = "";

            bool inMetaTag = false;
            bool inComment = false;
            bool inCDATA = false;

            XMLNodeList parents = new XMLNodeList();
            XMLNode currentNode = rootNode;

            for (int i = 0; i < content.Length; i++)
            {
                char c = content[i];
                char cn = '~';  // unused char
                char cnn = '~'; // unused char
                char cp = '~';  // unused char

                if ((i + 1) < content.Length) cn = content[i + 1];
                if ((i + 2) < content.Length) cnn = content[i + 2];
                if (i > 0) cp = content[i - 1];

                if (inMetaTag)
                {
                    if (c == QMARK && cn == RA)
                    {
                        inMetaTag = false;
                        i++;
                    }
                    continue;
                }
                else
                {
                    if (!quoted && c == LA && cn == QMARK)
                    {
                        inMetaTag = true;
                        continue;
                    }
                }

                if (inComment)
                {
                    if (cp == DASH && c == DASH && cn == RA)
                    {
                        inComment = false;
                        i++;
                    }
                    continue;
                }
                else
                {
                    if (!quoted && c == LA && cn == EXCLAMATION)
                    {
                        if (content.Length > i + 9 && content.Substring(i, 9) == "<![CDATA[")
                        {
                            inCDATA = true;
                            i += 8;
                        }
                        else
                        {
                            inComment = true;
                        }
                        continue;
                    }
                }

                if (inCDATA)
                {
                    if (c == SQR && cn == SQR && cnn == RA)
                    {
                        inCDATA = false;
                        i += 2;
                        continue;
                    }
                    textValue += c;
                    continue;
                }

                if (inElement)
                {
                    if (collectNodeName)
                    {
                        if (c == SPACE)
                        {
                            collectNodeName = false;
                        }
                        else if (c == RA)
                        {
                            collectNodeName = false;
                            inElement = false;
                        }

                        if (!collectNodeName && nodeName.Length > 0)
                        {
                            if (nodeName[0] == SLASH)
                            {
                                // close tag
                                if (textValue.Length > 0)
                                {
                                    currentNode.setText(currentNode.getText() + textValue);
                                }

                                textValue = "";
                                nodeName = "";
                                currentNode = parents.Pop();
                            }
                            else
                            {
                                if (textValue.Length > 0)
                                {
                                    currentNode.text += textValue;
                                }

                                textValue = "";
                                XMLNode newNode = new XMLNode(nodeName);
                                newNode.setText("");
                                XMLNodeList a = currentNode.get();
                                a.Push(newNode);
                                parents.Push(currentNode);
                                currentNode = newNode;
                                nodeName = "";
                            }
                        }
                        else
                        {
                            nodeName += c;
                        }
                    }
                    else
                    {
                        if (!quoted && c == SLASH && cn == RA)
                        {
                            inElement = false;
                            collectAttributeName = false;
                            collectAttributeValue = false;
                            if (attName.Length > 0)
                            {
                                if (attValue.Length > 0)
                                {
                                    if (currentNode.Select(attName) == null)
                                    {
                                        XMLNode att = new XMLNode(attName);
                                        currentNode.add(att);
                                        att.setText(attValue);
                                    }
                                    else
                                    {
                                        currentNode.Select(attName).setText(attValue);
                                    }
                                }
                                else
                                {
                                    if (currentNode.Select(attName) == null)
                                    {
                                        XMLNode att = new XMLNode(attName);
                                        currentNode.add(att);
                                        att.setText(attValue);
                                    }
                                    else
                                    {
                                        currentNode.Select(attName).setText("#t");
                                    }
                                }
                            }

                            i++;
                            currentNode = parents.Pop();
                            attName = "";
                            attValue = "";
                        }
                        else if (!quoted && c == RA)
                        {
                            inElement = false;
                            collectAttributeName = false;
                            collectAttributeValue = false;
                            if (attName.Length > 0)
                            {
                                if (currentNode.Select(attName) == null)
                                {
                                    XMLNode att = new XMLNode(attName);
                                    currentNode.add(att);
                                    att.setText(attValue);
                                }
                                else
                                {
                                    currentNode.Select(attName).setText(attValue);
                                }
                            }
                            attName = "";
                            attValue = "";
                        }
                        else
                        {
                            if (collectAttributeName)
                            {
                                if (c == SPACE || c == EQUALS)
                                {
                                    collectAttributeName = false;
                                    collectAttributeValue = true;
                                }
                                else
                                {
                                    attName += c;
                                }
                            }
                            else if (collectAttributeValue)
                            {
                                if (c == QUOTE || c == QUOTE2)
                                {
                                    if (quoted)
                                    {
                                        collectAttributeValue = false;
                                        if (currentNode.Select(attName) == null)
                                        {
                                            XMLNode att = new XMLNode(attName);
                                            currentNode.add(att);
                                            att.setText(attValue);
                                        }
                                        else
                                        {
                                            currentNode.Select(attName).setText(attValue);
                                        }

                                        attValue = "";
                                        attName = "";
                                        quoted = false;
                                    }
                                    else
                                    {
                                        quoted = true;
                                    }
                                }
                                else
                                {
                                    if (quoted)
                                    {
                                        attValue += c;
                                    }
                                    else
                                    {
                                        if (c == SPACE)
                                        {
                                            collectAttributeValue = false;
                                            XMLNode n = new XMLNode("@" + attName);
                                            n.setText(attValue);
                                            currentNode.get().Add(n);
                                            attValue = "";
                                            attName = "";
                                        }
                                    }
                                }
                            }
                            else if (c == SPACE)
                            {
                                //do nothing
                            }
                            else
                            {
                                collectAttributeName = true;
                                attName = "" + c;
                                attValue = "";
                                quoted = false;
                            }
                        }
                    }
                }
                else
                {
                    if (c == LA)
                    {
                        inElement = true;
                        collectNodeName = true;
                    }
                    else
                    {
                        textValue += c;
                    }
                }
            }
            return rootNode;
        }
    }
}
