#!/bin/bash

# Run tests with coverage
pytest --cov=app tests/ -v --cov-report=term-missing 