﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using CodeArt.Runtime;


namespace CodeArt.DomainDriven
{
    public abstract class PropertyValidator<TValue> : IPropertyValidator
    {
        public void Validate(IDomainObject domainObject, IDomainProperty property, ValidationResult result)
        {
            var obj = (DomainObject)domainObject;
            var pro = (DomainProperty)property;
            var propertyValue = obj.GetValue<TValue>(pro);
            Validate(obj, pro, propertyValue, result);
        }

        protected abstract void Validate(DomainObject domainObject, DomainProperty property, TValue propertyValue, ValidationResult result);
    }
    
    /// <summary>
    /// 非泛型版
    /// </summary>
    public abstract class PropertyValidator : IPropertyValidator
    {
        public void Validate(IDomainObject domainObject, IDomainProperty property, ValidationResult result)
        {
            var obj = (DomainObject)domainObject;
            var pro = (DomainProperty)property;
            var propertyValue = obj.GetValue(pro);
            Validate(obj, pro, propertyValue, result);
        }

        protected abstract void Validate(DomainObject domainObject, DomainProperty property, object propertyValue, ValidationResult result);
    }

}
