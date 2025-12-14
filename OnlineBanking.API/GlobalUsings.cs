global using System.Net;
global using AutoMapper;
global using MediatR;
global using Microsoft.AspNetCore.Mvc;

global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;
global using Microsoft.AspNetCore.Mvc.Filters;


global using Microsoft.Net.Http.Headers;
global using System.Text;
global using System.Text.Json;
global using FluentValidation;
global using Microsoft.OpenApi.Models;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Diagnostics;
global using Microsoft.AspNetCore.Authorization;

global using OnlineBanking.Core.Domain.Entities;
global using OnlineBanking.API.Extensions;
global using OnlineBanking.API.Middleware;
global using OnlineBanking.Application;
global using OnlineBanking.Infrastructure;
global using OnlineBanking.Core.Domain.Exceptions;

global using OnlineBanking.Application.Contracts.Infrastructure;
global using OnlineBanking.Application.Contracts.Persistence;
global using OnlineBanking.Application.Models.FastTransaction.Requests;
global using OnlineBanking.Application.Models.FastTransaction.Responses;

global using OnlineBanking.API.Filters;
global using OnlineBanking.Application.Helpers;
global using OnlineBanking.Application.Helpers.Params;
global using OnlineBanking.Application.Models.CreditCard;
global using OnlineBanking.Application.Models.CreditCard.Requests;
global using OnlineBanking.Application.Models.CreditCard.Responses;

global using OnlineBanking.Application.Enums;
global using OnlineBanking.Application.Models;
global using OnlineBanking.Application.Models.BankAccount;
global using OnlineBanking.Application.Models.BankAccount.Requests;
global using OnlineBanking.Application.Models.BankAccount.Responses;
global using OnlineBanking.Application.Models.CashTransaction.Requests;


global using OnlineBanking.Application.Models.Customer.Requests;
global using OnlineBanking.Application.Models.Customer.Responses;

global using OnlineBanking.API.Common;
global using OnlineBanking.API.Constants;

global using OnlineBanking.Application.Models.CashTransaction.Responses;
global using OnlineBanking.Core.Domain.Enums;

global using OnlineBanking.Application.Models.Branch.Requests;
global using OnlineBanking.Application.Models.Branch.Responses;

global using OnlineBanking.Application.Features.BankAccounts;
global using OnlineBanking.Application.Features.CashTransactions;