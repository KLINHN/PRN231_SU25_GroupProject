using AppCore;
using AppCore.Dtos;
using AppCore.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Repositories
{
    public interface ITestRepository
    {
        Task<TestDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<TestDto>> GetAll(CancellationToken cancellationToken = default);
        Task<bool> CreateAsync(TestDto testDto, Guid? creatorId = null, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(TestDto testDto, Guid? updaterId = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }

    public class TestRepository : ITestRepository
    {
        private readonly CrudRepository<Test> _testRepository;
        private readonly CrudRepository<Question> _questionRepository;
        private readonly CrudRepository<Answer> _answerRepository;

        public TestRepository(
            DbContext dbContext,
            IDbTransaction transaction)
        {
            _testRepository = new CrudRepository<Test>(dbContext, transaction);
            _questionRepository = new CrudRepository<Question>(dbContext, transaction);
            _answerRepository = new CrudRepository<Answer>(dbContext, transaction);
        }

        public async Task<TestDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var filter = new Expression<Func<Test, bool>>[]
            {
                x => x.Id == id
            };
            var entity = await _testRepository.FindOneAsync(filter, cancellationToken: cancellationToken);
            if (entity == null)
                return null;

            return new TestDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
            };
        }

        public async Task<List<TestDto>> GetAll(CancellationToken cancellationToken = default)
        {
            var entities = await _testRepository.GetAllAsync(cancellationToken: cancellationToken);
            return entities.Select(entity => new TestDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
            }).ToList();
        }

        public async Task<bool> CreateAsync(TestDto testDto, Guid? creatorId = null, CancellationToken cancellationToken = default)
        {
            var entity = new Test
            {
                Id = Guid.NewGuid(),
                Title = testDto.Title,
                Description = testDto.Description,
                CreatedAt = DateTime.UtcNow,
            };
            return await _testRepository.SaveAsync(entity, entity.Id, cancellationToken);
        }

        public async Task<bool> UpdateAsync(TestDto testDto, Guid? updaterId = null, CancellationToken cancellationToken = default)
        {
            var filter = new Expression<Func<Test, bool>>[]
            {
                x => x.Id == testDto.Id
            };
            var entity = await _testRepository.FindOneAsync(filter, cancellationToken: cancellationToken);
            if (entity == null)
                return false;

            entity.Title = testDto.Title;
            entity.Description = testDto.Description;

            return await _testRepository.SaveAsync(entity, entity.Id, cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _testRepository.HardDeleteAsync(id, cancellationToken);
        }
    }
}
