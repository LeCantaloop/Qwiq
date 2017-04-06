using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
  public class FieldCollection : IFieldCollection
  {
    private readonly IDictionary<int, IField> _cache;

    private readonly IRevisionInternal _revision;

    private readonly IFieldDefinitionCollection _definitions;

    private readonly Func<IRevisionInternal, IFieldDefinition, IField> _fieldFactory;

    internal FieldCollection(
      IRevisionInternal revision,
      IFieldDefinitionCollection definitions,
      Func<IRevisionInternal, IFieldDefinition, IField> fieldFactory)
    {
      _revision = revision;
      _definitions = definitions;
      _fieldFactory = fieldFactory;
      _cache = new Dictionary<int, IField>();
    }

    public virtual int Count => _definitions.Count;

    public virtual IField this[string name]
    {
      get
      {
        if (name == null) throw new ArgumentNullException(nameof(name));
        return GetById(_definitions[name].Id);
      }
    }

    public virtual bool Contains(string name)
    {
      try
      {
        return _definitions.Contains(name);
      }
      // REVIEW: Catch a more specific exception
      catch (Exception)
      {
        return false;
      }
    }

    public virtual IField GetById(int id)
    {
      if (!TryGetById(id, out IField byId))
      {
        throw new ArgumentException($"Field {id} does not exist.", nameof(id));
      }
      return byId;
    }

    public virtual IEnumerator<IField> GetEnumerator()
    {
      return _cache.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public virtual bool TryGetById(int id, out IField field)
    {
      if (_cache.TryGetValue(id, out field)) return true;
      try
      {
        if (_definitions.TryGetById(id, out IFieldDefinition def))
        {
          field = _fieldFactory(_revision, def);
          _cache[id] = field;
          return true;
        }
      }
      // REVIEW: Catch a more specific exception
      catch (Exception)
      {
      }
      return false;
    }
  }
}
