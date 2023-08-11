using System;
using AutoMapper;
using Refugio.DataBase;

namespace Refugio.Adapter
{
    public class BaseRepository
    {
        protected readonly Context _context;
        protected readonly IMapper _mapper;

        public BaseRepository(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}