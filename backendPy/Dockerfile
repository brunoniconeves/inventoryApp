FROM python:3.11-slim

WORKDIR /app

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY . .
# Install the package in development mode
RUN pip install -e .

# Add test command as an alternative entrypoint
COPY ./tests/run_tests.sh /run_tests.sh
RUN chmod +x /run_tests.sh

CMD ["uvicorn", "app.main:app", "--host", "0.0.0.0", "--port", "5000"] 