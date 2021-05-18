﻿using Business.Abstract;
using Business.Constant;
using Business.DependencyResolvers.AutoFac;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Validation;
using Core.Utilities.Results;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Concrete
{
    public class ProductManager : IProductService
    {
        IProductDal _productDal;

        public ProductManager(IProductDal productDal)
        {
            _productDal = productDal;
        }
        [SecuredOperation("product.add,admin")]
        [ValidationAspect(typeof(ProductValidator))]
        public IResult Add(Product product)
        {
            //business codes
            if (CheckIfProductCountOfCategoryCorrect(product.CategoryId).Success)
            {
                _productDal.Add(product);
                return new SuccessResult(Messages.ProductAdded);
            }
            return new ErrorResult();
        }

        public IResult Delete(Product product)
        {
            _productDal.Delete(product);
            return new SuccessResult(Messages.ProductAdded);
        }

        public IDataResult<Product> Get(int id)
        {
           var res =  _productDal.Get(p => p.ProductId == id);
            return new SuccessDataResult<Product>(res,Messages.ProductsListed);
        }

        public IDataResult<List<Product>> GetAll()
        {
            if (DateTime.Now.Hour == 22)
            {
                return new ErrorDataResult<List<Product>>(Messages.MaintenanceTime);
            }

            return new SuccessDataResult<List<Product>>(_productDal.GetAll(), Messages.ProductsListed);
        }

        public IDataResult<List<Product>> GetAllByCategoryId(int id)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(p => p.CategoryId == id));
        }

        public IDataResult<Product> GetById(int productId)
        {
            return new SuccessDataResult<Product>(_productDal.Get(p => p.ProductId == productId));
        }

        public IDataResult<List<Product>> GetByUnitPrice(decimal min, decimal max)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(p => p.UnitPrice >= min && p.UnitPrice <= max));
        }

        public IDataResult<List<ProductDetailsDto>> GetProductDetails()
        {
            if (DateTime.Now.Hour == 23)
            {
                return new ErrorDataResult<List<ProductDetailsDto>>(Messages.MaintenanceTime);
            }
            return new SuccessDataResult<List<ProductDetailsDto>>(_productDal.GetProductDetail());
        }
        [ValidationAspect(typeof(ProductValidator))]
        public IResult Update(Product product)
        {
            throw new NotImplementedException();
        }
        private IResult CheckIfProductCountOfCategoryCorrect(int categoryId)
        {
            var result = _productDal.GetAll(p => p.CategoryId == categoryId).Count;
            if (result >= 10)
            {
                return new ErrorResult(Messages.ProductNameInvalid);
            }
            return new SuccessResult();
        }
    }
}
