using System.Text.Json;

public class InboxWatcher : IDisposable
{
    private readonly string _inboxPath;
    private readonly string _processedPath;
    private readonly string _failedPath;
    private readonly OrderPipeline _pipeline;
    private readonly FileSystemWatcher _watcher;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(2);

    public InboxWatcher(string inboxPath, OrderPipeline pipeline)
    {
        _inboxPath = inboxPath;
        _pipeline = pipeline;

        _processedPath = Path.Combine(_inboxPath, "processed");
        _failedPath = Path.Combine(_inboxPath, "failed");

        Directory.CreateDirectory(_inboxPath);
        Directory.CreateDirectory(_processedPath);
        Directory.CreateDirectory(_failedPath);

        _watcher = new FileSystemWatcher(_inboxPath, "*.json");
        _watcher.Created += OnCreated;
        _watcher.EnableRaisingEvents = true;
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        _ = ProcessFileAsync(e.FullPath);
    }

    private async Task ProcessFileAsync(string filePath)
    {
        await _semaphore.WaitAsync();

        try
        {
            await Task.Delay(300);

            List<Order>? orders = null;
            Exception? lastException = null;

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    var json = await File.ReadAllTextAsync(filePath);

                    orders = JsonSerializer.Deserialize<List<Order>>(json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                    if (orders == null)
                    {
                        var singleOrder = JsonSerializer.Deserialize<Order>(json);
                        if (singleOrder != null)
                            orders = new List<Order> { singleOrder };
                    }

                    break;
                }
                catch (IOException ex)
                {
                    lastException = ex;
                    await Task.Delay(200);
                }
            }

            if (orders == null)
            {
                throw lastException ?? new Exception("Nie udało się odczytać pliku.");
            }

            foreach (var order in orders)
            {
                if (_pipeline.GetType().GetMethod("ProcessOrderAsync") != null)
                {
                    await (Task)_pipeline
                        .GetType()
                        .GetMethod("ProcessOrderAsync")!
                        .Invoke(_pipeline, new object[] { order })!;
                }
                else
                {
                    _pipeline.ProccessOrder(order);
                }
            }

            var destination = Path.Combine(
                _processedPath,
                Path.GetFileName(filePath)
            );

            if (File.Exists(destination))
                File.Delete(destination);

            File.Move(filePath, destination);
        }
        catch (Exception ex)
        {
            try
            {
                var failedFile = Path.Combine(
                    _failedPath,
                    Path.GetFileName(filePath)
                );

                if (File.Exists(failedFile))
                    File.Delete(failedFile);

                if (File.Exists(filePath))
                    File.Move(filePath, failedFile);

                await File.WriteAllTextAsync(
                    failedFile + ".error.txt",
                    ex.ToString()
                );
            }
            catch
            {
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        _watcher.EnableRaisingEvents = false;
        _watcher.Dispose();
        _semaphore.Dispose();
    }
}