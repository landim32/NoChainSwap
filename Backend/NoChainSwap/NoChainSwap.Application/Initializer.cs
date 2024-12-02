using Core.Domain;
using Core.Domain.Cloud;
using Core.Domain.Repository;
using DB.Infra;
using DB.Infra.Context;
using DB.Infra.Repository;
using NoChainSwap.Domain.Impl.Core;
using NoChainSwap.Domain.Impl.Factory;
using NoChainSwap.Domain.Impl.Services;
using NoChainSwap.Domain.Interfaces.Core;
using NoChainSwap.Domain.Interfaces.Factory;
using NoChainSwap.Domain.Interfaces.Models;
using NoChainSwap.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using NoChainSwap.Domain;
using NoChainSwap.Domain.Interfaces.Services.Coins;
using NoChainSwap.Domain.Impl.Services.Coins;

namespace NoChainSwap.Application
{
    public static class Initializer
    {

        private static void injectDependency(Type serviceType, Type implementationType, IServiceCollection services, bool scoped = true)
        {
            if(scoped)
                services.AddScoped(serviceType, implementationType);
            else
                services.AddTransient(serviceType, implementationType);
        }
        public static void Configure(IServiceCollection services, ConfigurationParam config, bool scoped = true)
        {
            if (scoped)
                services.AddDbContext<NoChainSwapContext>(x => x.UseLazyLoadingProxies().UseNpgsql(config.ConnectionString));
            else
                services.AddDbContextFactory<NoChainSwapContext>(x => x.UseLazyLoadingProxies().UseNpgsql(config.ConnectionString));

            StxTxService.WALLET_API = config.WalletStxApi;
            StxTxService.STACKS_API = config.StacksApi;

            #region Infra
            injectDependency(typeof(NoChainSwapContext), typeof(NoChainSwapContext), services, scoped);
            injectDependency(typeof(IUnitOfWork), typeof(UnitOfWork), services, scoped);
            injectDependency(typeof(ILogCore), typeof(LogCore), services, scoped);
            #endregion

            #region Repository
            injectDependency(typeof(IUserRepository<IUserModel, IUserDomainFactory>), typeof(UserRepository), services, scoped);
            injectDependency(typeof(IUserAddressRepository<IUserAddressModel, IUserAddressDomainFactory>), typeof(UserAddressRepository), services, scoped);
            injectDependency(typeof(ITransactionRepository<ITransactionModel, ITransactionDomainFactory>), typeof(TransactionRepository), services, scoped);
            injectDependency(typeof(ITransactionLogRepository<ITransactionLogModel, ITransactionLogDomainFactory>), typeof(TransactionLogRepository), services, scoped);
            #endregion

            #region Service
            injectDependency(typeof(IUserService), typeof(UserService), services, scoped);
            injectDependency(typeof(IMailerSendService), typeof(MailerSendService), services, scoped);
            injectDependency(typeof(ITransactionService), typeof(TransactionService), services, scoped);
            injectDependency(typeof(IBtcTxService), typeof(BtcTxService), services, scoped);
            injectDependency(typeof(IStxTxService), typeof(StxTxService), services, scoped);
            injectDependency(typeof(IMempoolService), typeof(MempoolService), services, scoped);
            injectDependency(typeof(ICoinMarketCapService), typeof(CoinMarketCapService), services, scoped);
            injectDependency(typeof(IStacksService), typeof(StacksService), services, scoped);
            #endregion

            #region Factory
            injectDependency(typeof(IUserDomainFactory), typeof(UserDomainFactory), services, scoped);
            injectDependency(typeof(IUserAddressDomainFactory), typeof(UserAddressDomainFactory), services, scoped);
            injectDependency(typeof(ITransactionDomainFactory), typeof(TransactionDomainFactory), services, scoped);
            injectDependency(typeof(ITransactionLogDomainFactory), typeof(TransactionLogDomainFactory), services, scoped);
            injectDependency(typeof(ICoinTxServiceFactory), typeof(CoinTxServiceFactory), services, scoped);
            #endregion


            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, AuthHandler>("BasicAuthentication", null);

        }
    }
}
