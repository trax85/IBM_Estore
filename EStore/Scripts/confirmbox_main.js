jQuery(document).ready(function($){
	//open popup
	$('.cd-popup-trigger').on('click', function(event){
		event.preventDefault();
		$('.cd-popup').addClass('is-visible');
	});

	$('.cd-popup-trigger-list').on('click', function (event) {
		event.preventDefault();
		const item = $(this).data('item-id');
		const url = $(this).data('item-url');
		$('.cd-popup').addClass('is-visible');
		
		// Setup the delete confirmation action
		$('.cd-popup-confirm').on('click', function (event) {
			event.preventDefault();

			// Perform the deletion action (navigate to the delete URL)
			window.location.href = url + encodeURIComponent(item);
		});

		// Setup the cancel action
		$('.cd-popup-cancel').on('click', function (event) {
			event.preventDefault();

			// Hide the popup
			$('.cd-popup').removeClass('is-visible');
		});
	});
	
	//close popup
	$('.cd-popup').on('click', function(event){
		if( $(event.target).is('.cd-popup-close') || $(event.target).is('.cd-popup') ) {
			event.preventDefault();
			$(this).removeClass('is-visible');
		}
	});
	//close popup
	$('.cd-popup').on('click', function (event) {
		if ($(event.target).is('.cd-popup-close-btn') || $(event.target).is('.cd-popup')) {
			event.preventDefault();
			$(this).removeClass('is-visible');
		}
	});
	//close popup when clicking the esc keyboard button
	$(document).keyup(function(event){
    	if(event.which=='27'){
    		$('.cd-popup').removeClass('is-visible');
	    }
    });
});