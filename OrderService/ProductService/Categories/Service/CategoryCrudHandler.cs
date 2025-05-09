﻿using Core;
using Infrastructure;

namespace ProductService.Categories.Service
{
    public class CategoryCrudHandler(
        IBaseValidator<Category> _validator,
        ICrudRepository<Category> _repo
    ) : CrudHandlerBase<Category>(_validator, _repo), ICrudHandler<Category>
    { }
}
