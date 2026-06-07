# TravelBook

TravelBook is a desktop WPF application written in C# for planning trips.
The application allows users to manage cities, attractions, travel routes, and trip expenses in one place.

## Features

### Cities

Users can:

* add new cities;
* edit existing cities;
* delete cities;
* view city details;
* search cities by name or country.

Each city contains:

* name;
* country;
* description;
* image file path;
* list of attractions.

### Attractions

Each city can have its own list of attractions.

Users can:

* add attractions to a city;
* edit attractions;
* delete attractions;
* filter attractions by category;
* search attractions by name or address.

Attraction fields:

* name;
* category;
* address;
* entry price;
* rating from 1 to 5.

Available attraction categories:

* Museum;
* Park;
* Restaurant;
* Hotel;
* Other.

### Routes

Users can create travel routes using previously added cities.

The route module allows users to:

* create a new route;
* add cities to the route;
* move cities up or down to change the order;
* remove cities from the route;
* save routes to a JSON file;
* view saved routes;
* search routes by name, city, or country.

### Budget

Users can manage the trip budget and expenses.

The budget module allows users to:

* set the total trip budget;
* add expenses;
* delete expenses;
* view the total amount spent;
* view the remaining budget;
* filter expenses by category;
* search expenses by title or category;
* view simple category statistics.

Expense categories:

* Housing;
* Transport;
* Food;
* Attractions;
* Other.

## Technologies Used

* C#
* .NET 10
* WPF
* System.Text.Json
* Three-layer architecture:

  * Models
  * Logic
  * UI

## Project Structure

```text
TravelBook
│
├── data
│   ├── cities.json
│   ├── routes.json
│   └── budget.json
│
├── Models
│   ├── City.cs
│   ├── Attraction.cs
│   ├── AttractionCategory.cs
│   ├── TravelRoute.cs
│   ├── Budget.cs
│   ├── Expense.cs
│   └── ExpenseCategory.cs
│
├── Logic
│   ├── JsonStorage.cs
│   ├── CityService.cs
│   ├── RouteService.cs
│   └── BudgetService.cs
│
├── UI
│   ├── MainWindow.xaml
│   ├── MainWindow.xaml.cs
│   ├── CitiesPage.xaml
│   ├── CitiesPage.xaml.cs
│   ├── CityDetailPage.xaml
│   ├── CityDetailPage.xaml.cs
│   ├── CityEditDialog.xaml
│   ├── CityEditDialog.xaml.cs
│   ├── AttractionEditDialog.xaml
│   ├── AttractionEditDialog.xaml.cs
│   ├── RoutePage.xaml
│   ├── RoutePage.xaml.cs
│   ├── BudgetPage.xaml
│   └── BudgetPage.xaml.cs
│
├── App.xaml
├── App.xaml.cs
└── TravelBook.csproj
```

## Data Storage

TravelBook stores data in JSON files inside the `data` folder.

```text
data/cities.json
```

Stores cities and their attractions.

```text
data/routes.json
```

Stores saved travel routes.

```text
data/budget.json
```

Stores the total budget and expenses.

The application uses `System.Text.Json` for reading and writing data.

## How to Run the Project

### Requirements

Make sure you have installed:

* Visual Studio 2022 or newer;
* .NET 10 SDK;
* Windows operating system.

### Steps

1. Open the solution or project folder in Visual Studio.
2. Wait until Visual Studio restores the project dependencies.
3. Select the `TravelBook` project as the startup project.
4. Press `F5` or click the green `Start` button.
5. The TravelBook desktop application will open.

## How to Use TravelBook

### Main Menu

After launching the application, use the main navigation menu to switch between modules:

* Cities;
* Routes;
* Budget.

### Managing Cities

1. Open the `Cities` section.
2. Click `Add City`.
3. Enter the city name, country, description, and image path.
4. Save the city.
5. The city will appear in the cities list.

To edit or view a city, click on it in the list.

To search for a city, type its name or country in the search field.

### Managing Attractions

1. Open a city from the cities list.
2. Click `Add Attraction`.
3. Fill in the attraction information:

   * name;
   * category;
   * address;
   * entry price;
   * rating.
4. Save the attraction.

Use the category filter to display only attractions of a selected type.

Use the search field to find attractions by name or address.

### Creating a Route

1. Open the `Routes` section.
2. Click `New Route`.
3. Enter the route name.
4. Select a city from the dropdown list.
5. Click `Add`.
6. Repeat the process to add more cities.
7. Use `Move Up` and `Move Down` to change the city order.
8. Click `Save` to save the route.

Saved routes are stored in `data/routes.json`.

### Managing Budget

1. Open the `Budget` section.
2. Enter the total trip budget.
3. Click `Apply`.
4. Add expenses using the expense form.
5. Select the expense category, amount, and date.
6. Click `Add Expense`.

The application will automatically calculate:

* total budget;
* total spent;
* remaining budget.

Use search and category filters to find specific expenses.

## Notes

* If the application cannot build because `TravelBook.exe` is locked, close the running TravelBook window or stop debugging in Visual Studio using `Shift + F5`.
* The `data` folder must remain in the project directory because the application uses it to store JSON files.
* Image paths are stored as file paths, so images should not be moved after being added unless the path is updated.

## Author

TravelBook was created as a WPF desktop application project for learning C#, JSON file storage, and layered application architecture.
