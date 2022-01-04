using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace MecWise.Blazor.Components {
    public class FieldCollection : ICollection<Field> {
        public bool Render { get; set; } = false;

        public List<Field> FieldList { 
            get {
                return this.GetFieldList();
            } 
        }

        private List<Field> _fields;

        public FieldCollection() {
            _fields = new List<Field>();
        }

        public int Count => _fields.Count;

        public bool IsReadOnly => false;

        public void Add(Field item) {
            _fields.Add(item);
        }

        public void Clear() {
            _fields.Clear();
        }

        public bool Contains(Field item) {
            return _fields.Contains(item);
        }

        public void CopyTo(Field[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public IEnumerator<Field> GetEnumerator() {
            return _fields.GetEnumerator();
        }

        public bool Remove(Field item) {
            return _fields.Remove(item);
        }



        IEnumerator IEnumerable.GetEnumerator() {
            return _fields.GetEnumerator();
        }


        private List<Field> GetFieldList() {
            List<Field> fieldList = new List<Field>();

            foreach (Field field in _fields) {
                fieldList.Add(field);

                if (field is FieldTabContainer) {
                    FieldTabContainer tabContainer = (FieldTabContainer)field;
                    List<Field> resultFields = GetListFromTabContainer(tabContainer);
                    fieldList.AddRange(resultFields);
                }
                else if (field is FieldContainer) {
                    FieldContainer container = (FieldContainer)field;
                    List<Field> resultFields = GetListFromContainer(container);
                    fieldList.AddRange(resultFields);
                }
            }
            return fieldList;
        }

        private List<Field> GetListFromContainer(FieldContainer parentField) {
            List<Field> fieldList = new List<Field>();

            foreach (Field field in parentField.Fields) {
                fieldList.Add(field);

                if (field is FieldTabContainer) {
                    FieldTabContainer tabContainer = (FieldTabContainer)field;
                    List<Field> resultFields = GetListFromTabContainer(tabContainer);
                    fieldList.AddRange(resultFields);
                }
                else if (field is FieldContainer) {
                    FieldContainer container = (FieldContainer)field;
                    List<Field> resultFields = GetListFromContainer(container);
                    fieldList.AddRange(resultFields);
                }
            }
            return fieldList;
        }

        private List<Field> GetListFromTabContainer(FieldTabContainer parentField) {
            List<Field> fieldList = new List<Field>();

            foreach (FieldTabPage page in parentField.TabPages) {
                foreach (Field field in page.Fields) {
                    fieldList.Add(field);

                    if (field is FieldTabContainer) {
                        FieldTabContainer tabContainer = (FieldTabContainer)field;
                        List<Field> resultFields = GetListFromTabContainer(tabContainer);
                        fieldList.AddRange(resultFields);
                    }
                    else if (field is FieldContainer) {
                        FieldContainer container = (FieldContainer)field;
                        List<Field> resultFields = GetListFromContainer(container);
                        fieldList.AddRange(resultFields);
                    }
                }
            }
            return fieldList;
        }

        public T Search<T>(string searchFieldId) {
            Field field = this.Search(searchFieldId);
            if (field != null) {
                if (field is T) {
                    //return (T)Convert.ChangeType(field, typeof(T));
                    return (T)(object)field;
                }
            }

            return default(T);
        }


        public Field Search(string searchFieldId) {
            foreach (Field field in _fields) {
                if (field is FieldTabContainer) {
                    if (field.ID == searchFieldId)
                        return field;

                    FieldTabContainer tabContainer = (FieldTabContainer)field;
                    Field resultField = SearchInTabContainer(tabContainer, searchFieldId);
                    if (resultField != null)
                        return resultField;
                }
                else if (field is FieldContainer) {
                    if (field.ID == searchFieldId)
                        return field;

                    FieldContainer container = (FieldContainer)field;
                    Field resultField = SearchInContainer(container, searchFieldId);
                    if (resultField != null)
                        return resultField;
                }
                else {
                    if (field.ID == searchFieldId)
                        return field;
                }
            }
            return null;
        }

        private Field SearchInContainer(FieldContainer parentField, string searchFieldId) {
            foreach (Field field in parentField.Fields) {
                if (field is FieldTabContainer) {
                    if (field.ID == searchFieldId)
                        return field;

                    FieldTabContainer tabContainer = (FieldTabContainer)field;
                    Field resultField = SearchInTabContainer(tabContainer, searchFieldId);
                    if (resultField != null)
                        return resultField;
                }
                else if (field is FieldContainer) {
                    if (field.ID == searchFieldId)
                        return field;

                    FieldContainer container = (FieldContainer)field;
                    Field resultField = SearchInContainer(container, searchFieldId);
                    if (resultField != null)
                        return resultField;
                }
                else {
                    if (field.ID == searchFieldId)
                        return field;
                }
            }
            return null;
        }

        private Field SearchInTabContainer(FieldTabContainer parentField, string searchFieldId) {
            foreach (FieldTabPage page in parentField.TabPages) {
                foreach (Field field in page.Fields) {
                    if (field is FieldTabContainer) {
                        if (field.ID == searchFieldId)
                            return field;

                        FieldTabContainer tabContainer = (FieldTabContainer)field;
                        Field resultField = SearchInTabContainer(tabContainer, searchFieldId);
                        if (resultField != null)
                            return resultField;
                    }
                    else if (field is FieldContainer) {
                        if (field.ID == searchFieldId)
                            return field;

                        FieldContainer container = (FieldContainer)field;
                        Field resultField = SearchInContainer(container, searchFieldId);
                        if (resultField != null)
                            return resultField;
                    }
                    else {
                        if (field.ID == searchFieldId)
                            return field;
                    }
                }
            }
            return null;
        }
    }

}


