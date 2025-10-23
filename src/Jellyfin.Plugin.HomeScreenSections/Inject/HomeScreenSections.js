'use strict';

if (typeof HomeScreenSectionsHandler == 'undefined') {
    const DISCOVER_SEARCH_QUERY_KEY = 'homeScreenSectionsDiscoverSearchQuery';
    const SEARCH_INPUT_ID = 'searchTextInput';
    const SEARCH_APPLY_MAX_ATTEMPTS = 20;
    const SEARCH_APPLY_DELAY = 200;

    const HomeScreenSectionsHandler = {
        init: function() {
            var MutationObserver = window.MutationObserver || window.WebKitMutationObserver;
            var myObserver = new MutationObserver(this.mutationHandler);
            var observerConfig = {childList: true, characterData: true, attributes: true, subtree: true};

            $("body").each(function () {
                myObserver.observe(this, observerConfig);
            });

            HomeScreenSectionsHandler.applyHandlersToExistingDiscoverCards(document.body);
            HomeScreenSectionsHandler.applyStoredSearchQueryIfNeeded();

            window.addEventListener('hashchange', function () {
                setTimeout(function () {
                    HomeScreenSectionsHandler.applyStoredSearchQueryIfNeeded();
                }, 50);
            });
        },
        mutationHandler: function (mutationRecords) {
            mutationRecords.forEach(function (mutation) {
                if (mutation.addedNodes && mutation.addedNodes.length > 0) {
                    [].forEach.call(mutation.addedNodes, function (addedNode) {
                        HomeScreenSectionsHandler.applyHandlersToExistingDiscoverCards(addedNode);
                    });
                }
            });
        },
        applyHandlersToExistingDiscoverCards: function(rootNode) {
            if (!rootNode || !rootNode.nodeType) {
                return;
            }

            if (rootNode.nodeType !== 1 && rootNode.nodeType !== 9 && rootNode.nodeType !== 11) {
                return;
            }

            var $root = $(rootNode);

            if ($root.hasClass('discover-card')) {
                HomeScreenSectionsHandler.bindDiscoverCardEvents($root);
            }

            $root.find('.discover-card').each(function () {
                HomeScreenSectionsHandler.bindDiscoverCardEvents($(this));
            });
        },
        bindDiscoverCardEvents: function($card) {
            if ($card.data('discoverHandlersBound')) {
                return;
            }

            $card.on('click', '.discover-requestbutton', HomeScreenSectionsHandler.clickHandler);
            $card.on('click', '.discover-search-link', HomeScreenSectionsHandler.searchClickHandler);
            $card.data('discoverHandlersBound', true);
        },
        clickHandler: function(event) {
            window.ApiClient.ajax({
                url: window.ApiClient.getUrl("HomeScreen/DiscoverRequest"),
                type: "POST",
                data: JSON.stringify({
                    UserId: window.ApiClient._currentUser.Id,
                    MediaType: $(this).data('media-type'),
                    MediaId: $(this).data('id'),
                }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json'
            }).then(function(response) {
                if (response.errors && response.errors.length > 0) {
                    Dashboard.alert("Item request failed. Check browser logs for details.");
                    console.error("Item request failed. Response including errors:");
                    console.error(response);
                } else {
                    Dashboard.alert("Item successfully requested");
                }
            }, function(error) {
                Dashboard.alert("Item request failed");
            })
        },
        searchClickHandler: function(event) {
            event.preventDefault();
            event.stopPropagation();

            var $card = $(event.currentTarget).closest('.discover-card');
            if (!$card || $card.length === 0) {
                return;
            }

            var encodedTitle = $card.attr('data-discover-title') || '';
            var title = '';

            try {
                title = decodeURIComponent(encodedTitle);
            } catch (e) {
                title = encodedTitle;
            }

            HomeScreenSectionsHandler.navigateToSearchWithTitle(title);
        },
        navigateToSearchWithTitle: function(title) {
            if (!title) {
                return;
            }

            var trimmedTitle = title.trim();
            if (!trimmedTitle) {
                return;
            }

            sessionStorage.setItem(DISCOVER_SEARCH_QUERY_KEY, trimmedTitle);

            var searchUrl = HomeScreenSectionsHandler.getSearchPageUrl(trimmedTitle);

            if (!searchUrl) {
                return;
            }

            if (HomeScreenSectionsHandler.isOnSearchPage()) {
                if (window.location.href !== searchUrl) {
                    window.location.href = searchUrl;
                } else {
                    setTimeout(function () {
                        HomeScreenSectionsHandler.applyStoredSearchQueryIfNeeded();
                    }, 50);
                }
            } else {
                window.location.href = searchUrl;
            }
        },
        getSearchPageUrl: function(query) {
            var currentHref = window.location.href || '';
            var webIndex = currentHref.indexOf('/web/');
            var basePath;

            if (webIndex !== -1) {
                basePath = currentHref.substring(0, webIndex + 5) + '#/search';
            } else {
                basePath = '/web/#/search';
            }

            if (!query) {
                return basePath;
            }

            return basePath + '?query=' + HomeScreenSectionsHandler.encodeQueryForSearch(query);
        },
        encodeQueryForSearch: function(query) {
            return encodeURIComponent(query).replace(/%20/g, '+');
        },
        isOnSearchPage: function() {
            if (!window.location || !window.location.hash) {
                return false;
            }

            return window.location.hash.indexOf('/search') !== -1;
        },
        applyStoredSearchQueryIfNeeded: function() {
            if (!HomeScreenSectionsHandler.isOnSearchPage()) {
                return;
            }

            var storedQuery = sessionStorage.getItem(DISCOVER_SEARCH_QUERY_KEY);
            if (!storedQuery) {
                return;
            }

            var tryApply = function() {
                var searchInput = document.getElementById(SEARCH_INPUT_ID);
                if (!searchInput) {
                    return false;
                }

                if (searchInput.value !== storedQuery) {
                    searchInput.value = storedQuery;
                    var inputEvent = new Event('input', { bubbles: true, cancelable: true });
                    searchInput.dispatchEvent(inputEvent);
                }

                sessionStorage.removeItem(DISCOVER_SEARCH_QUERY_KEY);
                return true;
            };

            if (tryApply()) {
                return;
            }

            var attempts = 0;
            var intervalId = setInterval(function() {
                attempts++;

                if (tryApply() || attempts >= SEARCH_APPLY_MAX_ATTEMPTS) {
                    clearInterval(intervalId);

                    if (attempts >= SEARCH_APPLY_MAX_ATTEMPTS) {
                        sessionStorage.removeItem(DISCOVER_SEARCH_QUERY_KEY);
                    }
                }
            }, SEARCH_APPLY_DELAY);
        }
    };

    $(document).ready(function () {
        setTimeout(function () {
            HomeScreenSectionsHandler.init();
        }, 50);
    });
}

if (typeof TopTenSectionHandler == 'undefined') {
    const TopTenSectionHandler = {
        init: function () {
            var MutationObserver = window.MutationObserver || window.WebKitMutationObserver;
            var myObserver = new MutationObserver(this.mutationHandler);
            var observerConfig = {childList: true, characterData: true, attributes: true, subtree: true};

            $("body").each(function () {
                myObserver.observe(this, observerConfig);
            });
        },
        mutationHandler: function (mutationRecords) {
            mutationRecords.forEach(function (mutation) {
                if (mutation.addedNodes && mutation.addedNodes.length > 0) {
                    [].some.call(mutation.addedNodes, function (addedNode) {
                        if ($(addedNode).hasClass('card')) {
                            if ($(addedNode).parents('.top-ten').length > 0) {
                                var index = parseInt($(addedNode).attr('data-index'));
                                $(addedNode).attr('data-number', index + 1);
                            }
                        }
                    });
                }
            });
        }
    }

    setTimeout(function () {
        TopTenSectionHandler.init();
    }, 50);
}
