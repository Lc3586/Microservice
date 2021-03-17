using Business.Interface.System;
using Castle.DynamicProxy;
using Microservice.Library.Container;
using System;

namespace Business.Utils.Filter
{
    /// <summary>
    /// 仅限管理员
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AdministratorOnlyAttribute : BaseFilterAttribute
    {
        IOperator _Operator;

        /// <summary>
        /// 当前登录人
        /// </summary>
        protected IOperator Operator
        {
            get
            {
                if (_Operator == null)
                    _Operator = AutofacHelper.GetScopeService<IOperator>();
                return _Operator;
            }
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="invocation"></param>
        public override void OnActionExecuting(IInvocation invocation)
        {
            if (Operator.IsAuthenticated && !Operator.IsAdmin)
                throw new ApplicationException("无权限");
        }

        /// <summary>
        /// 执行后
        /// </summary>
        /// <param name="invocation"></param>
        public override void OnActionExecuted(IInvocation invocation)
        {

        }
    }
}
