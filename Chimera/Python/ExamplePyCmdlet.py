def greetings(name, timeOfDay): # greetings will only get called from ExampleCmdlet.cs
    # Time of Day:
    # 1 - Morning, 2 - Afternoon, 3 - Evening.  Any other value is replaced with "Day"
    greeting = None
    if timeOfDay == 1:
        greeting = "Morning"
    elif timeOfDay == 2:
        greeting = "Afternoon"
    elif timeOfDay == 3:
        greeting = "Evening"
    else:
        greeting = "Day"
    value = "Good {0} {1}".format(greeting, name)
    return value
    
print("hello world from ExamplePyCmdlet") # Gets called when loaded. Ideal for some scenarios