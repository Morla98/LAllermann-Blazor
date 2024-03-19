# TODO
## Chapter 1: Login and Registration
- [X] Implement user registration page
- [X] UserAuthenticationService SignOut (even if not needed per se)
- [X] Implement UserAuthenticationService.RefreshTokenIfNeeded 
- [X] Implement that if the cookie is expired you are actually logged out (or check why it currently still isnt working)
- [X] Use HttpOnly Cookies and configure AuthProperties with usefull settings
- [X] Test if token needs multiple Roles (Like User and Admin) instead of Admins can automatically see User Content
- [X] lookup refresh token (Will not be used for now)
- [ ] Lookup SignInManager
- [ ] Decide for How and When to refresh the token (Undecided until rough PasswordManager implementation)
- [ ] Codereview and Cleanup

## Chapter 2: General Layout / Design
- [ ] Decide on a general layout
- [ ] Implement a general layout
- [ ] To be continued...

## Chapter 3: PasswordManager
- [ ] ...

## Bugfixes
- [ ] Bugfix: calling /refresh while not logged in throws a nullpointer exception

## Optional
- [ ] Implement a User Panel
- [ ] Implement a Admin Panel
- [ ] Think about a better way to refresh the token in InteractiveServer Rendermode