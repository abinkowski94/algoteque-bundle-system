
# Algoteque Bundle System

This repository contains the implementation of a course bundle recommendation system that solves a coding assignment for KK, an education platform. The goal of the system is to generate resource bundle quotes based on teacher requests for various topics. This application matches the topics requested by teachers with available providers and calculates bundle quotes using specific pricing rules.

## Problem Overview

The coding assignment involves building an application that returns bundle quotes based on teacher requests for specific course topics. Each teacher request specifies the level of content needed for various topics, and the system should calculate quotes from different resource providers based on their available topics and pricing approach.

### Scenario

Teachers can request different amounts of content coverage for topics such as reading, math, science, history, and art. The system matches the teacher’s request with available topics from resource providers and calculates bundle quotes based on the following rules:

- If two topics match with a provider's offerings, the quote is 10% of the combined requested content for those topics.
- If only one topic matches, the quote is based on the importance of the topic:
  - 20% for the highest requested topic.
  - 25% for the second-highest topic.
  - 30% for the third-highest topic.

The system should only return quotes if the calculated value is greater than zero.

## Example (Previous Version with Incorrect Calculation)

In the previous version of the implementation, there were a couple of errors in the example provided. The following was the calculation:

Given the teacher request:

```json
{
  "topics": {
    "reading": 20,
    "math": 50,
    "science": 30,
    "history": 15,
    "art": 10
  }
}
```

The system incorrectly calculated the following quotes:

- **provider_a**: 8 (10% of 80 for two matches on math and science) – Correct.
- **provider_b**: 5 (25% of 20 for one match on reading, the 3rd biggest topic) – Incorrect. *provider_b* actually matches **two** topics: reading and science.
- **provider_c**: 12.5 (25% of 50 for one match on math, the 2nd biggest topic) – Incorrect. *provider_c* matches **math** (the highest requested topic), so the correct quote should be based on 20%.

## Example (Corrected Version)

The corrected logic for the calculations is as follows:

Given the same teacher request:

```json
{
  "topics": {
    "reading": 20,
    "math": 50,
    "science": 30,
    "history": 15,
    "art": 10
  }
}
```

The top 3 requested topics are:
- **math**: 50
- **science**: 30
- **reading**: 20

The system will now correctly calculate the following quotes for each provider:

- **provider_a**: 8 (10% of 80 for two matches on math and science) – Correct.
- **provider_b**: 5 (10% of 50 for two matches on reading and science) – Corrected.
- **provider_c**: 10 (20% of 50 for one match on math, the highest requested topic) – Corrected.

### Key Details

- **Static Configuration**: The system uses a static JSON configuration to define which topics each provider offers.
- **Top 3 Topics**: The application only considers the top 3 requested topics for each teacher's request, disregarding others.
- **Zero Quote Elimination**: If the calculated quote is zero, the system will not return a quote for that provider.

### Pricing Model

1. **Two Topic Matches**: If a provider offers two matching topics, the quote is 10% of the combined requested content for those topics.
2. **One Topic Match**: If only one topic matches, the quote depends on the ranking of the topics requested by the teacher:
   - 20% for the highest requested topic.
   - 25% for the second-highest topic.
   - 30% for the third-highest topic.

### Example of Provider Configurations

```json
{
  "provider_topics": {
    "provider_a": "math+science",
    "provider_b": "reading+science",
    "provider_c": "history+math"
  }
}
```

## Project Structure

- `AlgotequeBundleSystem/`: Contains the main application code that implements the bundle system.
  - `BundleService.cs`: The core service that processes the teacher’s request and calculates quotes based on the provided business logic.
  - `ProviderService.cs`: A service that handles the mapping between providers and their available topics.
- `Tests/`: Contains unit tests that validate the behavior of the system.
  - `BundleServiceTests.cs`: Unit tests for the `BundleService` class.
  - `ProviderServiceTests.cs`: Unit tests for the `ProviderService` class.
- `Config/`: Holds static configuration files, including the provider topics configuration in JSON format.

## How to Run the Application

1. Clone the repository:
   ```bash
   git clone https://github.com/abinkowski94/algoteque-bundle-system.git
   cd algoteque-bundle-system
   ```

2. Open the solution file in Visual Studio or your preferred .NET IDE.

3. Restore the required NuGet packages:
   ```bash
   dotnet restore
   ```

4. Build the solution:
   ```bash
   dotnet build
   ```

5. Run the application:
   ```bash
   dotnet run
   ```

6. You can modify the `teacher_request.json` and `provider_config.json` files to test different scenarios.

## Notes on the Coding Assignment

- The example in the assignment contains a few errors that were addressed in the implementation. The business logic, as defined in the project, follows the correct pricing rules.
- The focus is on clear, maintainable, and well-structured code rather than a fully comprehensive solution. Therefore, edge cases or more complex scenarios have not been explored extensively in this implementation.
- The solution adheres to object-oriented principles, with the key services handling business logic and configuration separately.

## Assessment Criteria

- **Code Structure and Readability**: The code is organized into services that follow the single responsibility principle, and each class or method has a clear purpose.
- **Object-Oriented Design**: The design uses classes, encapsulation, and methods to logically separate concerns, such as bundle calculation and provider handling.
- **Testing**: The application includes unit tests to validate the core functionality, ensuring that the calculations are accurate based on the provided request.
