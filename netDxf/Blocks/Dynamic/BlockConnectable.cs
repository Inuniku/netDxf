using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic
{
    public class BlockConnection
    {
        public int Id { get; set; }
        public string Connection { get; set; }
        public bool IsValid => Id != 0;

        internal object Evaluate(BlockEvaluationContext context)
        {
            BlockEvalConnectable expr = context.EvalGraph.GetNode(Id) as BlockEvalConnectable;
            
            if(expr != null)
            {
                return expr.GetConnectionValue(Connection);
            }

            throw new InvalidOperationException("Connection not setup");
        }
    }

    public abstract class BlockEvalConnectable : EvalExpr
    {
        protected Dictionary<string, Func<object>> connectionGetterDict = new Dictionary<string, Func<object>>();
        protected Dictionary<string, Action<object>> connectionSetterDict = new Dictionary<string, Action<object>>();
        protected Dictionary<string, Type> connectionType = new Dictionary<string, Type>();


        protected BlockEvalConnectable(string codename) : base(codename)
        {
            foreach (var propInfo in GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                ConnectablePropertyAttribute att = propInfo.GetCustomAttribute<ConnectablePropertyAttribute>();
                if (att != null)
                {
                    MethodInfo getMethod = propInfo.GetMethod;
                    MethodInfo setMethod = propInfo.SetMethod;

                    if (att.VectorType.HasFlag(ConnectableVectorType.Normal))
                    {
                        if (propInfo.GetMethod != null)
                        {
                            connectionGetterDict.Add(att.PropertyName + att.Postfix, () => getMethod.Invoke(this, Array.Empty<object>()));
                            connectionType.Add(att.PropertyName + att.Postfix, propInfo.PropertyType);
                        }

                        if (propInfo.SetMethod != null)
                            connectionSetterDict.Add(att.PropertyName + att.Postfix, (value) => propInfo.SetMethod.Invoke(this, new object[] { value }));
                    }

                    if (att.VectorType.HasFlag(ConnectableVectorType.X) ||
                        att.VectorType.HasFlag(ConnectableVectorType.Y) ||
                        att.VectorType.HasFlag(ConnectableVectorType.Z))
                    {
                        if (propInfo.PropertyType != typeof(Vector3) && propInfo.PropertyType != typeof(Vector2))
                        {
                            throw new InvalidOperationException("Only Vector3 or Vector 2 objects are valid componentGetter");
                        }

                        if (getMethod != null)
                        {

                            if (att.VectorType.HasFlag(ConnectableVectorType.X))
                            {
                                connectionGetterDict.Add(att.PropertyName + "X" + att.Postfix, () => ((Vector3)getMethod.Invoke(this, Array.Empty<object>())).X);
                                connectionType.Add(att.PropertyName + "X" + att.Postfix, typeof(double));
                            }

                            if (att.VectorType.HasFlag(ConnectableVectorType.Y))
                            {
                                connectionGetterDict.Add(att.PropertyName + "Y" + att.Postfix, () => ((Vector3)getMethod.Invoke(this, Array.Empty<object>())).Y);
                                connectionType.Add(att.PropertyName + "Y" + att.Postfix, typeof(double));
                            }

                            if (att.VectorType.HasFlag(ConnectableVectorType.Z))
                            {
                                connectionGetterDict.Add(att.PropertyName + "Z" + att.Postfix, () => ((Vector3)getMethod.Invoke(this, Array.Empty<object>())).Y);
                                connectionType.Add(att.PropertyName + "Z" + att.Postfix, typeof(double));
                            }

                            if (propInfo.SetMethod != null)
                            {
                                if (att.VectorType.HasFlag(ConnectableVectorType.X))
                                {
                                    connectionSetterDict.Add(att.PropertyName + "X" + att.Postfix, (value) =>
                                    {
                                        Vector3 vector = (Vector3)getMethod.Invoke(this, Array.Empty<object>());
                                        vector.X = (double)value;
                                        setMethod.Invoke(this, new object[] { value });
                                    });

                                }

                                if (att.VectorType.HasFlag(ConnectableVectorType.Y))
                                {
                                    connectionSetterDict.Add(att.PropertyName + "Y" + att.Postfix, (value) =>
                                    {
                                        Vector3 vector = (Vector3)getMethod.Invoke(this, Array.Empty<object>());
                                        vector.Y = (double)value;
                                        setMethod.Invoke(this, new object[] { value });
                                    });
                                }

                                if (att.VectorType.HasFlag(ConnectableVectorType.Z))
                                {
                                    connectionSetterDict.Add(att.PropertyName + "Z" + att.Postfix, (value) =>
                                    {
                                        Vector3 vector = (Vector3)getMethod.Invoke(this, Array.Empty<object>());
                                        vector.Z = (double)value;
                                        setMethod.Invoke(this, new object[] { value });
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }

        public object GetConnectionValue(string connectionName)
        {
            return connectionGetterDict[connectionName].Invoke();
        }

        public void SetConnectionValue(string connectionName, object value)
        {
            connectionSetterDict[connectionName].Invoke(value);
        }

        public bool HasConnection(string connectionName, bool includeSetter = false)
        {
            bool hasGetter = connectionGetterDict.ContainsKey(connectionName);
            if (includeSetter)
                return connectionSetterDict.ContainsKey(connectionName) && hasGetter;

            return hasGetter;
        }

        public IEnumerable<string> GetConnections()
        {
            return connectionGetterDict.Keys;
        }
    }
}
