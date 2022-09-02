
using Existence.Grpc.Protos;
using Grpc.Core;

namespace Existence.Grpc.Services
{
    public class ExistenceService: ProductExistence.ProductExistenceBase
    {

        void hola() 
        {
        
        }
        public async override Task<CheckExistenceReponse> 
            CheckExistence(CheckExistenceRequest request, ServerCallContext context)
        {
            await Task.Delay(1);

            return new CheckExistenceReponse { ProductQTY = 89 };
        }
    }
}
