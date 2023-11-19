using Core.Domain.Entities;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services.Interface;

namespace Sieve.Services
{
    public class ThesisSieveProcessor : SieveProcessor
    {
        public ThesisSieveProcessor(
            IOptions<SieveOptions> options,
            ISieveCustomSortMethods customSortMethods,
            ISieveCustomFilterMethods customFilterMethods)
            : base(options, customSortMethods, customFilterMethods)
        {
        }

        protected override SievePropertyMapper MapProperties(SievePropertyMapper mapper)
        {
            mapper.Property<Thesis>(p => p.LecturerThesis.Name)
                .CanFilter();

            return mapper;
        }
    }
}
