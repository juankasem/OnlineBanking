
global using System.Globalization;
global using System.Linq.Expressions;
global using System.Reflection;


global using AutoMapper;
global using MediatR;
global using FluentValidation;
global using FluentValidation.AspNetCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;

global using OnlineBanking.Application.Contracts.Infrastructure;
global using OnlineBanking.Application.Contracts.Persistence;
global using OnlineBanking.Application.Enums;
global using OnlineBanking.Application.Helpers;
global using OnlineBanking.Application.Helpers.Params;
global using OnlineBanking.Application.Models;
global using OnlineBanking.Application.Models.CashTransaction;
global using OnlineBanking.Application.Models.BankAccount;
global using OnlineBanking.Application.Features.BankAccounts;
global using OnlineBanking.Application.Features.CashTransactions.Commands;
global using OnlineBanking.Application.Features.CashTransactions.Queries;
global using OnlineBanking.Application.Features.Customers;
global using OnlineBanking.Application.Mappings.BankAccounts;
global using OnlineBanking.Application.Mappings.Branches;
global using OnlineBanking.Application.Mappings.CashTransactions;
global using OnlineBanking.Application.Mappings.CreditCards;

global using OnlineBanking.Core.Domain.Enums;
global using OnlineBanking.Core.Domain.Services.BankAccount;
global using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

global using OnlineBanking.Application.Models.BankAccount.Responses;

global using OnlineBanking.Application.Common.Helpers;
global using OnlineBanking.Core.Domain.Constants;

global using OnlineBanking.Application.Features.FastTransactions.Commands;
global using OnlineBanking.Application.Features.FastTransactions.Messages;