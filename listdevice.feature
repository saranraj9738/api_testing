Feature: List All Devices

  Scenario: Successfully get all devices from the collection
    Given the baseurl and geturl are ready
    When the user requests the list of all devices
    Then the status code should be 200
