Feature: Add New Device

  Scenario Outline: Add a phone successfully with a valid price
    Given the baseurl and posturl are ready
    When the user sends the devicename "<DeviceName>" and price <Price>
    Then the status code must shown 200

    Examples: 

      | DeviceName      | Price |
      | Apple iPhone 15 | 10000 |
      | Google Pixel 8  | 20000 |

  Scenario Outline: Try to add a phone with an price in words
    Given the baseurl and posturl are ready
    When the user sends the devicename "<DeviceName>" and text price "<Price>"
    Then the status code must shown 200

    Examples: 

      | DeviceName      |Price|
      | Apple iPhone 15 | twenty thousand|
      | Google Pixel 8  | ten thousand|
