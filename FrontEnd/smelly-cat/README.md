# SmellyCat



### External Tools & Libraries:
  1. Nominatim - Open-source geocoding with OpenStreetMap data

     Used for address lookup and contact form autocomplete location fields
     
     URL: https://nominatim.org/

  2. Leaflet - Open-source JavaScript library for interactive maps

     Used for creating address map in contact page

     URL: https://leafletjs.com/

  3. EmailJS - Server-Less Email Sender

     Used for mailing out copies of the contact form submittion

     URL: https://www.emailjs.com/

### Webpage Styling Tools:
  1. SCSS
  2. Tailwind CSS

### Testing:

  There are 22 tests implemented checking various aspects and edge-case scenarios safe-guarding and limiting bugs and errors

  #### ContactComponent
  
  1. should create
  
  #### Form Validation
  
  2. should require email field and validate email format
  
  3. should require fullName field
  
  4. should have optional city, postalCode, address, and message fields
  
  5. should be valid when all required fields are filled
  
  6. should require terms checkbox to be checked
  
  7. should initialize with an invalid form
  
  #### Form Submission
  
  8. should prevent double submission
  
  9. should show success message and reset form on successful submission
  
  10. should disable form during submission
  
  11. should call emailjs.send with correct parameters on valid submission
  
  12. should not submit if form is invalid
  
  13. should show error message and re-enable form on failed submission
  
  #### Address Autocomplete
  
  14. should trigger search after debounce delay for input with 3+ characters
  
  15. should display suggestions when search returns results
  
  #### UI Interactions
  
  16. should hide suggestions after delay when hideSuggestionsWithDelay is called
  
  #### Suggestion Selection
  
  17. should extract house number from current input if not in suggestion
  
  18. should handle municipality with "Municipality of" prefix
  
  19. should populate form fields when suggestion is selected
  
  20. should hide suggestions after selection
  
  21. should not trigger search for short input (less than 3 characters)
  
  22. should hide suggestions when no results returned
