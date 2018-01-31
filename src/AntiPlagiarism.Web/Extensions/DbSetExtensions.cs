﻿using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AntiPlagiarism.Web.Extensions
{
	public static class DbSetExtensions
	{
		public static EntityEntry<TEntity> AddOrUpdate<TEntity>(this DbSet<TEntity> dbSet, TEntity entity, Expression<Func<TEntity, bool>> findFunction) where TEntity : class
		{
			var exists = dbSet.AsNoTracking().Any(findFunction);
			return exists ? dbSet.Update(entity) : dbSet.Add(entity);
		}
	}
}